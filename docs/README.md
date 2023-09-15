# Environment
- **Visual Studio 2022** and extensions:
    - *Markdown Editor v2* is a full featured Markdown editor with live preview and syntax highlighting. 
	- *CodeMaid VS2022* is helping with the cleanup, organize, and simplify your code.
	- *SVG Viewer* makes it much easier to work with SVG files by showing a live preview in the bottom-right corner of the code window.
- **SQL Server Management Studio v18.6**
- **Docker Desktop v4.3.2**

## Setup DevExpress.Blazor NuGet package
1. Navigate to https://nuget.devexpress.com/. 
2. Log in or create DevExpress account (use your Getinge e-mail).
3. Click "Obtain Feed URL".
4. Copy DevExpress NuGet Feed URL: https://nuget.devexpress.com/our_unique_identifier/api.
5. Setup package source in Microsoft Visual Studio: 
    - Go to: Tools -> Options -> NuGet Package Manager -> Package Sources.
	- Add new package source: Name='DevExpress-Blazor' and Source='https://nuget.devexpress.com/our_unique_identifier/api'.


# Setup User Secrets
To start using 'User Secrets', you can do the following steps in Visual Studio:
1. Right-click on the `ProductionService.API` and select 'Manage User Secrets'. It opens the file `secrets.json`.
2. Add the following to `secrets.json`:
	```
	"ConnectionStrings": {
		"TDocContext": "data source=host.docker.internal;initial catalog=RoadrunnerTest;integrated security=False;User ID=TDOC;Password=<my-db-password>;MultipleActiveResultSets=true"
	}
	```
3. Replace `<my-db-password>` with your password for your `TDOC` SQL login to the test database. If you don't have a `TDOC` SQL login/user, you can create them by using the following script:
	```
	CREATE LOGIN [TDOC] WITH PASSWORD = '<my-db-password>', CHECK_POLICY = OFF, CHECK_EXPIRATION=OFF, DEFAULT_DATABASE=[master];
	USE [RoadrunnerTest]
	CREATE USER [TDOC] FOR LOGIN [TDOC];
	EXEC sp_addrolemember N'db_owner', N'TDOC';
	```
4. Repeat the above steps for `SearchService.API`.


# Project structure

## Shared projects that are exposed like NuGet packages

*Important to keep in mind*: it is not allowed to make changes to the source of the shared projects - unless it has been agreed with the owner of the shared projects. The reason is that it can affect multiple T-DOC applications. 

1. Project **TDOC.EntityFramework** (NuGet package "TDOC.EntityFramework")
	- EntityFramework DB context.
	- Key id generator.
	- DTO (data transfer objects) models.
	- EntityFramework models.
	- Model to DB mapper.
	- TDOC constants and enumerations.

	*Dependencies*:		
        
       -Micosoft.EntityFrameworkCore 
       -Microsoft.EntityFrameworkCore.SqlServer

2. Project **Common** (NuGet package "TDOC.Common")
	- DB context and SQL utilities.
	- Sequence number generator.
	- Stream operations for reading from a textual and binary database field.
	- Functionality for incremental reading of textual and binary data from a Sql Server database.
	- DTO models.
	- Extension methods for IQueryable, strings and authentication.
	- Common constants and enumerations.
	- Utilities for bit, string, file date and json.

	*Notes*: In general this project keeps classes that are applicable for both client side and server side.<br>
	*Dependencies*: 

       -Dapper 
	   -System.Data.SqlClient 
	   -TDOC.Entites

3. Project **Common.Client** (NuGet package "TDOC.Common.Client")
	- Functionality for performing asynchronous HTTP requests with strongly typed serialization and deserialization of request and response content.

	*Notes*: In general this project keeps classes that are applicable only for client side.<br>
	*Dependencies*: 

		-Microsoft.AspNet.WebApi.Client 
		-Microsoft.Extensions.Http

4. Project **Common.Server** (NuGet package "TDOC.Common.Server")
	- Authentication claimsprincipal extensions for getting user key id.
	- Base API controller.
   	- Dependency injection setup as extension methods containing database, identity, localization, mvc and versioning setup.
	- HTTP header keys.
	- Supported cultures for localization.
	- Swagger configuration.
    - Models for Validation errors.
    - Middleware configuration for ApiException handling and database transaction erros.
	*Notes*: In general this project keeps classes that are applicable only for server side.<br>
	*Dependencies*: 

        -TDOC.Common 
	    -Microsoft.AspNetCore.Authentication.JwtBearer 
        -Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer 
        -Swashbuckle.AspNetCore

5. Project **TDOC.WebComponents** (NuGet package "TDOC.WebComponents")
	- MediaSwiper component.

	*Notes*: This project keeps Blazor UI components to be used in our application.<br>
	*Dependencies*: 
	
		-TDOC.Common.

## Own Roadrunner projects

1. Project **ProductionService.Shared**
	- DTO objects, models constants and enumerations strictly belong to Roadrunner.

	*Dependencies*: 

	   -TDOC.Common

2. Project **ProductionService.Core**
	- *Business rules of the application*: Service interfaces and implementations.
	- Repository interfaces.
	
	*Dependencies*: ProductionService.Shared.

3. Project **ProductionService.Infrastructure**
	- *Interactions with entities/DB*: Repository implementations.

	*Dependencies*: 

	   -ProductionService.Core

4. Project **ProductionService.API**
	- *Interface adapter*: API and Authentication controllers.
	- AutoMappers - profiles for DTOs and models.

	*Dependencies*: 

	   -TDOC.Common.Server
       -ProductionService.Infrastructure

5. Project **ScannerClient.WebApp**
	- "wwwroot" with index.html, css, js files.
	- Blazor feature-pages.
	- Services to execute API calls.

	*Dependencies*: 

	   -TDOC.Common.Client
       -TDOC.WebComponents
       -ProductionService.Shared

6. Project **DBScripts**
	- Create and upgrade database scripts

## Test projects

- ProductionService.Core.Test (xUnit - .NET Core)
- ProductionService.Infrastructure.Test (xUnit - .NET Core)
- ScannerClient.WebApp.Test (bUnit - .NET Core)


# Architectural concepts for **ScannerWeb.Client** project. 

For every new *FeatureName* follow these rules: 
- Each **feature** goes into its own folder in the root into "ScannerWeb.Client" project. All code related to that feature should be placed there. This tries to emulate the "one module per feature" guideline.
- Services re-used throughout the entire Blazor application should be placed in the **Core folder**.
- Re-usable components (usually components without routing, but not always) should be placed in the **Shared folder**.
- The **markup** and **C# code** should be separated into different files (FeatureName.razor and FeatureName.razor.cs).

>Resources:<br>
http://danpatrascu.com/architecting-blazor-applications-an-angular-approach/<br>
https://mariomucalo.com/organizing-and-naming-components-in-blazor/


# HTTP error codes
 
- *HTTP 200* if request DTO is valid. Response DTO may still include business or domain validation Error (e.g. When user requested to pick particular Unit, but for this Unit expired date exceeded, then validation Error should be returned with "Unit expiry date exceeded" message). So, for sending validation Errors to Client we should wrap the Response DTO by **Foundation.Core.Service.Result<>**.

		public async Task<ActionResult<Result<OrderBaseInformation>>> CreateOrderAsync([FromBody]CreateOrderArgs createOrderArgs)
		{
			...
			return result;
		}

- *HTTP 400* if validation of Request DTO fails (e.g. Pure analyzing of Request DTO properties without including DB checks. It means that Client didn't specify correctly some properties of Request DTO).
- *HTTP 404* if referenced resource does not exist (e.g. When user request concrete Item or Product by KeyId). Note: not the same as empty list result (e.g. When user request list of Items or Products to be returned)).
- *HTTP 500* if Server fails to execute a valid request. And root cause is not on the client or user side. For this type of error code we have centralized Error Middleware.


# Sql server connection configuration
  TDocEFDbContext is injected with connectionstring stored in appsettings.json  
           
    services.AddDbContext<TDocEFDbContext>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString("TDocContext"));
                option.ConfigureWarnings(
                    warnings => { warnings.Ignore(CoreEventId.ForeignKeyAttributesOnBothNavigationsWarning); }
                );
            });
 
Connectionstring is stored as a simple path in below scheme
  
    "ConnectionStrings": {
      "TDocContext": "data source=host.docker.internal;initial catalog=RoadrunnerTest;integrated security=False;User ID=sa;Password=Qwer1234;MultipleActiveResultSets=true"
     }
    
Initiating in Test Projects
       
    public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            return config;
        }
        private static string InitConnectionString()
        {
            if (!string.IsNullOrEmpty(_connectionString))
                return _connectionString;
            _connectionString = InitConfiguration().GetConnectionString("TDocContext");
            return _connectionString;
        }

# Localization

To localize your "FeatureName" page/component do next:
1. Inside folder "Resources" create new sub-folder "FeatureName".
2. Inside just created sub-folder add new empty class "FeatureNameResource.cs".

		namespace ClientProject.Resources.FeatureName
		{
			public class FeatureNameResource
			{
			}
		}

3. Now you can add Resource files: *FeatureNameResource.en-US.resx*, *FeatureNameResource.fr-FR.resx*, ... And add required translations for all the terms that are using on "FeatureName" page/component.
4. Sample how to use localization on *FeatureName.razor* page/component:

		@inject IStringLocalizer<ClientProject.Resources.FeatureName.FeatureNameResource> localizer
		...
		<h1>@localizer["someTerm"]</h1>

>Resources:<br>
https://docs.microsoft.com/en-us/aspnet/core/blazor/globalization-localization?view=aspnetcore-3.1


# CSS isolation and Excubo.WebCompiler

## Install Excubo.WebCompiler
A common mistake is to add the package Excubo.WebCompiler as a nuget package reference to a project (e.g. by installing it via the nuget package manager in Visual Studio). This does not work! Instead, one needs to install it as a dotnet tool.
It's already done by adding the next script in ScannerClient.WebApp.csproj:
- for Visual Studio (Windows OS):

		<Target Name="PreBuild" BeforeTargets="PreBuildEvent" Condition="'$(OS)' == 'Windows_NT'">
			<Exec Command="dotnet tool install Excubo.WebCompiler --global" ContinueOnError="true" />
		</Target>
- for Docker (Linux OS):

		<Target Name="PreBuildDocker" BeforeTargets="PreBuildEvent" Condition="'$(OS)' != 'Windows_NT'">
			<Exec Command="dotnet new tool-manifest --force" />
			<Exec Command="dotnet tool install Excubo.WebCompiler" />
			<Exec Command="dotnet tool restore" />
			<Exec Command="dotnet tool webcompiler -h" />
		</Target>
As the result every time before ScannerClient.WebApp project is built the latest version of Excubo.WebCompiler is installed and all the SCSS-files declared in compilerFiles.conf/compilerFiles.conf.tomove are recompiled, but only if the SCSS-files have been changed after the last compilation.

## Use CSS isolation
To use CSS isolation for "FeatureName" page/component follow these steps:
1. Create new "FeatureName.razor.scss".
2. Add file path to compilerFiles.conf or compilerFiles.conf.tomove depends on the path, where the resulting CSS file should be generated.</br>
If the resulting CSS file should be generated in the same folder as "FeatureName.razor.scss" - add file path to compilerFiles.conf file.</br>
If the resulting CSS file should be generated in "wwwroot\css\generated" folder:
   - add file path to compilerFiles.conf.tomove file
   - add the related script for moving the resulting CSS file to "wwwroot\css\generated" folder for both configurations Visual Studio and Docker:</br>

			<!-- Visual Studio related part -->
			...
			<Target Name="CopyCssFiles" AfterTargets="CompileStaticAssets" Condition="'$(OS)' == 'Windows_NT'">
				...
				<!-- Copying generated CSS-files to 'wwwroot/css/generated' directory-->
					...
					<Exec Command="xcopy /y File\Path\*.css wwwroot\css\generated" />
				<!--Removing generated CSS-files from origins -->
					...
					<Exec Command="del File\Path\*.css" />
			</Target>
			...
			<!-- Docker related part -->
			...
			<Target Name="PreBuildDocker" BeforeTargets="PreBuildEvent" Condition="'$(OS)' != 'Windows_NT'">
				...
				<!-- Copying generated CSS-files to 'wwwroot/css/generated' directory-->
					...
					<Exec Command="cp -v File/Path/*.css wwwroot/css/generated" />
				<!--Removing generated CSS-files from origins -->
					...
					<Exec Command="rm -f File/Path/*.css" />
			</Target>

>Resources:<br>
https://sass-lang.com/documentation/at-rules/mixin <br>
https://github.com/excubo-ag/WebCompiler <br>
https://chrissainty.com/get-some-sass-into-your-blazor-app/

# Static assets minification & bundling

## Create custom bundle for devextreme (dx.all.js)
In general we do not need to modify the custom bundle unless we want to use another feature or widget in devextreme bundle.
To create a custom bundle using the DevExtreme Bundler tool, you need Webpack, TerserWebpackPlugin, and the DevExtreme package installed globally using node package manager.

	npm install -g webpack  
	npm install -g terser-webpack-plugin  
	npm install -g devextreme 
Create the DevExtreme configuration using this command (Already exist in TDoc.WebComponents project, you just need to uncomment the features you want to add.).

	devextreme-bundler-init dx.custom
The following command produces a minified bundle which you can replace with the one in project.

	devextreme-bundler dx.custom

## Can we select stylings as well instead of using the whole theme (dx.light.css)?
Sure its possible. When you install devextreme modules using npm the default directory exists in
"C:\Users\[Your username]\AppData\Roaming\npm\node_modules".
Within this address you should create a sass file with required styles to be included. Template for this configuration file
you can find in TDOC.WebComponents/wwwroot/lib/devextreme/css/dx.light.custom.config.scss. 	
Then the easiest part is compiling this created sass:	

	sass dx.light.custom.config.scss dx.light.custom.min.css compressed
The "compressed" phrase in command will create the minified version.
Now just replace the generated css with the one in project.

## What about other contents?
In case you need to add new stylesheets or javascript files into TDoc.WebComponents project
there's no need to minify it manually or even link it to index.html.
The new added file needs to be defined in "bundleconfig.json" existing file. Don't worry about the rest. 
It's already embedded in build pipeline so the bundle.min.js & bundle.min.css will be creates everytime automatically.

>Resources:<br>
https://js.devexpress.com/Documentation/Guide/Common/Modularity/Create_a_Custom_Bundle/ <br>
https://github.com/DevExpress/DevExtreme/issues/13654 <br>
https://github.com/madskristensen/BundlerMinifier <br>

# Store state based on Redux pattern (Fluxor)

The whole global state of your app is stored in an object tree inside a single *store*. The only way to change the state tree is to create an *action*, an object describing what happened, and *dispatch* it to the store. To specify how state gets updated in response to an action, you write pure *reducer* functions that calculate a new state based on the old state and the action.

In the case you need to remember state of "FeatureName" objects or variables and share it all over the other features, you should follow these: 
1. Inside "FeatureName" folder create sub-folder and call it "Store".
2. Inside "Store" folder you can include such files:
	- *FeatureNameActions.cs*<br>
		The events that occur in the app based on user input, and trigger updates in the State. You can think of an action as an event that describes something that happened in the application.
		In other words: a metadata container containing information about what the user did and what our application should do after the user has performed the interaction with our page to kick off our flux pipeline.
	- *FeatureNameEffects.cs*<br>
		An Effect is an object that contains some information to be interpreted by the middleware. You can view Effects like instructions to the middleware to perform some operation (e.g., invoke some asynchronous function, dispatch an action to the store, etc.).
		In other words: as subscribers to dispatched actions, think about effects as listeners of specific actions, performing resulting tasks based on what action was just issued.
	- *FeatureNameFeature.cs*<br>
		This class describes the State to the Store. 
		Descending from *Feature<FeatureNameState>*, this tells Fluxor at startup (during its initial assembly scanning *ScanAssemblies*) that this will be a store feature named *FeatureName* that will house a *FeatureNameState* object.
	- *FeatureNameReducers.cs*<br>
		Simple pure methods whose only job is to take state in, and spit state out by way of non-destructive mutation, i.e. taking our current state object in, examining what action was just dispatched and how the state should be transformed, and spitting out a new state object with said transformations
	- *FeatureNameState.cs*
		 A snapshot of the currently rendered page, containing any and all data our application is concerned about at that point in time.
3. *Dispatching* an Action to indicate our intention to change State:

		[Inject]		
		private readonly IDispatcher _dispatcher;
		...
		_Dispatcher.Dispatch(new FeatureNameAction());
	
	Note: *dispatcher* is nothing more than a request delegator of sorts, this guy/gal is in charge of issuing actions anytime a user decides to do something on our page.

4. Read data from Store:

		[Inject]
        private IState<FeatureNameState> _featureNameState { get; set; }
		...
		foreach (var someItem in _featureNameState.Value.SomeList)
		{
		...
		}

	Note: *store* is the central piece, our store is the state container, holding on to all current slices of state that all components ultimately subscribe to and react to any store changes accordingly when the store notifies listeners there�s been an update.

>Resources:<br>
https://github.com/mrpmorris/Fluxor/blob/master/Tutorials/02-Blazor/02A-StateActionsReducersTutorial/README.md<br>
https://betweentwobrackets.dev/posts/2020/06/state-management-with-blazor-using-fluxor-part-1/<br>
https://www.youtube.com/watch?v=sAyH-O0dFaI<br>
https://www.youtube.com/watch?v=k_c-ErPaYa8


# Transform Source object in DTO object (AutoMapper)

AutoMapper is an object-object mapper. Object-object mapping works by transforming an input object of one type into an output object of a different type.

We are using AutoMapper to transform *Entity Framework model* into *DTO model* to hide sensitive properties, to prepare calculated properties and so on.

To use AutoMapper for new transformation one object into another follow these steps:
1. Add new file "FeatureName.cs" into "Shared" project inside "Data" folder.
2. Add new DTO file "FeatureNameDTO.cs" into "Shared" project inside "DTO" folder.
3. Use one of two ways to register transformation of one object into another:
	- *Define Profile*: open "Server" project and add new file "FeatureNameMapperProfile.cs" inside "MapperProfiles" folder.

			public class FeatureNameMapperProfile : Profile
			{
				public FeatureNameMapperProfile()
				{
					// Next settings will map the following properties to each other: property_name->PropertyName
					SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
					DestinationMemberNamingConvention = new PascalCaseNamingConvention();

					// Next settings for ignoring defined prefixes when performing transformation
					ClearPrefixes();
					RecognizePrefixes("pref1", "pref2");

					CreateMap<FeatureName, FeatureNameDTO>()
						.ForMember(dest => dest.DestProperty, opt => opt.MapFrom(src => src.SrcProperty))
						.ForMember(dest => dest.CalculatedProperty, opt => opt.MapFrom(src => src.SrcProperty > 100));
				}
			}

		Note: the AddAutoMapper method provided by the AutoMapper package will traverse the assembly and checks for the class which inherits from the Profile class of AutoMapper.

			services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); 

	- *Define MapperConfiguration*: open any implementation method of Repository class and initialize MapperConfiguration:

			private MapperConfiguration _config;

			public FeatureNameRepository(...)
			{
				_config = new MapperConfiguration(cfg => cfg.CreateMap<FeatureName, FeatureNameDTO>());
			}

		Call mapper method to transform one object into another:

			return _context.FeatureNames.ProjectTo<FeatureNameDTO>(_config).ToList();

>Resources:<br>
https://docs.automapper.org/en/latest/Setup.html<br>
https://github.com/AutoMapper/AutoMapper


# Call JavaScript from .NET and vise verse

Note: **directly modifying the DOM with JavaScript isn't recommended** in most scenarios because JavaScript can interfere with Blazor's change tracking.

>Resources:<br>
https://docs.microsoft.com/en-us/aspnet/core/blazor/call-javascript-from-dotnet?view=aspnetcore-3.1<br>
https://docs.microsoft.com/en-us/aspnet/core/blazor/call-dotnet-from-javascript?view=aspnetcore-3.1


# Debuging

To debug in Chrome browser run this command:<br>

	chrome --remote-debugging-port=9222 --user-data-dir="C:\Users\user-name\AppData\Local\Temp\blazor-chrome-debug" https://localhost:5001/

Note: known issues:
- Sometimes it is unable to fetch script source (this is for files located in tool Chrome/Debug/Source).
- Cannot visualize every single variables in debug mode in Chrome DevTools (for example: DateTime).
- Cannot break on unhandled exceptions.
- Cannot hit breakpoints during app startup before the debug proxy is running. This includes breakpoints in Program.Main (Program.cs) and breakpoints in the OnInitialized{Async} methods of components that are loaded by the first page requested from the app.

>Resources:<br>
https://docs.microsoft.com/en-us/aspnet/core/blazor/debug?view=aspnetcore-3.1&tabs=visual-studio<br>
https://itnext.io/debugging-blazor-web-assembly-apps-c47ef25bcb5f


# Build and run Roadrunner server & client inside docker

1. Open Command Prompt (cmd.exe) like administrator.
2. Change path to be *cd M:\Roadrunner\build*.
3. Update existing file *.env* to match your db user name and password:<br>
````DB_SERVER=host.docker.internal````<br>
````DB_CATALOG=RoadrunnerTest````<br>
````DB_USER=[your_db_user_name]````<br>
````DB_PASSWORD=[your_db_user_password]````
4. To build all docker images use this command:<br>
````docker-compose build --build-arg USERNAME=[your_user_name] --build-arg PAT=[your_personal_access_tocken] --build-arg DEUSERNAME=DevExpress --build-arg DEPAT=[your_feed_authorization_key]````
5. To build docker image only for *APIGateway*:<br>
````docker build -f APIGateway/Docker/Dockerfile.Development -t roadrunner-gateway````
6. To build docker image only for *ProductionService.API*:<br>
````docker build -f ProductionService/ProductionService.API/Docker/Dockerfile.Development -t roadrunner-production . --build-arg USERNAME=[your_user_name] --build-arg PAT=[your_personal_access_tocken]````
7. To build docker image only for *SearchService.API*:<br>
````docker build -f SearchService/SearchService.API/Docker/Dockerfile.Development -t roadrunner-search . --build-arg USERNAME=[your_user_name] --build-arg PAT=[your_personal_access_tocken]````
8. To build docker image only for *MasterDataService.API*:<br>
````docker build -f MasterDataService/MasterDataService.API/Docker/Dockerfile.Development -t roadrunner-master-data . --build-arg USERNAME=[your_user_name] --build-arg PAT=[your_personal_access_tocken]````
9. To build docker image only for *ScannerClient.WebApp*:<br>
````docker build -f ScannerClient/ScannerClient.WebApp/Docker/Dockerfile.Development -t roadrunner-scanner-client . --build-arg USERNAME=[your_user_name] --build-arg PAT=[your_personal_access_tocken] --build-arg DEUSERNAME=DevExpress --build-arg DEPAT=[your_feed_authorization_key]````
10. To build docker image only for *AdminClient.WebApp*:<br>
````docker build -f AdminClient/AdminClient.WebApp/Docker/Dockerfile.Development -t roadrunner-admin-client . --build-arg USERNAME=[your_user_name] --build-arg PAT=[your_personal_access_tocken] --build-arg DEUSERNAME=DevExpress --build-arg DEPAT=[your_feed_authorization_key]````
11. To run docker containers for all *Roadrunner* images:<br>
````docker-compose up -d````

Note:<br> 
To run service's swagger(s) using api gateway navigate to:
- ````[GatewayUrl]/production/swagger-api/index.html````
- ````[GatewayUrl]/search/swagger-api/index.html````
- ````[GatewayUrl]/master-data/swagger-api/index.html````

To run client(s) using api gateway navigate to:
- ````[GatewayUrl]/scanner-client````
- ````[GatewayUrl]/admin-client````


Note:<br>
Personal access token (PAT) should be generated for your account in Azure DevOps with "Packaging: Read, write, & manage" rights. For creating PAT:<br>
- Log into https://dev.azure.com/GetingeDHS
- Click on your picture profile in the right upper corner and select **Security** menu
- Select **Personal access tockens** option on the left side menu and click **+ New Token** button
- In the opened **Create a new personal access token** popup dialog input **Name**, 
select needed **Expiration (UTC)** (1 year max), 
**Custom defined** scopes and tick **Read, write, & manage** under **Packaging** scope
- Click **Create** button
- *"You have successfully added a new personal access token. Copy the token now!"* message is displayed. 
Your PAT is generated and displayed under this message in the textbox. This PAT should be inserted in Docker build arguments 
Remember: the generated PAT will never be displayed again after closing the dialog, 
so your need to copy and save it somewhere else before closing the dialog.<br>

Note:<br>
How to obrtain DevExpress feed credenntials:
https://docs.devexpress.com/GeneralInformation/116042/installation/install-devexpress-controls-using-nuget-packages/obtain-your-nuget-feed-credentials

>Resources:<br>
https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-5.0<br>
https://chrissainty.com/containerising-blazor-applications-with-docker-containerising-a-blazor-webassembly-app/


# Logging (Serilog)

We have possibility to log different levels of LogEvent (Verbose, Debug, Information, Warning, Error, Fatal). To change it, open "Server" project and set an appropriate log level in *Program.cs*.

We have different write-places where we can display/store logs:

	...
	.WriteTo.Console()
	.WriteTo.File("log\\exception.txt")

>Resources:<br>
https://github.com/nblumhardt/serilog-sinks-browserhttp/tree/dev/samples<br>
https://github.com/Scotty-Hudson/BlazorWasmLogUnhandledExceptions/tree/master/BlazorWasmLogUnhandledExceptions


# Exception handling

Framework and app code may trigger unhandled exceptions in any of the following locations:
- Component instantiation
- Lifecycle methods
- Rendering logic
- Event handlers
- Component disposal
- JavaScript interop
- Blazor Server rerendering

Note: we are using centralized place to handle all exceptions *UnhandledExceptionProvider*.

>Resources:<br>
https://docs.microsoft.com/en-us/aspnet/core/blazor/fundamentals/handle-errors?view=aspnetcore-3.1<br>
https://stackoverflow.com/questions/57539330/is-there-a-way-to-globally-catch-all-unhandled-errors-in-a-blazor-single-page-ap


# Unit testing

## The scenario when choosing **unit testing to perform**

- Component without JS interop logic.<br>
	When there's no dependency on JS interop in a Blazor component, the component can be tested without access to JS or the DOM API. In this scenario, there are no disadvantages to choosing unit testing.
- Component with simple JS interop logic.<br>
	It's common for components to query the DOM or trigger animations through JS interop. Unit testing is usually preferred in this scenario, since it's straightforward to mock the JS interaction through the IJSRuntime interface.
- Component that depends on complex JS code.<br>
	If a component uses JS interop to call a large or complex JS library but the interaction between the Blazor component and JS library is simple, then the best approach is likely to treat the component and JS library or code as two separate parts and test each individually. Test the Blazor component with a unit testing library, and test the JS with a JS testing library.
- Note: use **bUnit** to write Blazor unit tests.

## Use **code coverage** for unit testing

1. Navigate to "Test" project folder and and run the dotnet test command 

		cd M:\Roadrunner\ProductionService\ProductionService.Core.Test 
		dotnet test --collect:"XPlat Code Coverage"

	As part of the *dotnet test* run, a resulting *coverage.cobertura.xml* file is output to the *TestResults* directory. 

2. Now that you're able to collect data from unit test runs, you can generate reports using *ReportGenerator*. To install the *ReportGenerator* NuGet package as a .NET global tool, use the dotnet tool install command

		dotnet tool install -g dotnet-reportgenerator-globaltool

3. Run the tool and provide the desired options, given the output *coverage.cobertura.xml* file from the previous test run.

		reportgenerator -reports:"M:\Roadrunner\ProductionService\ProductionService.Core.Test\TestResults\{guid}\coverage.cobertura.xml" -targetdir:"M:\Roadrunner\ProductionService\ProductionService.Core.Test\TestResults\{guid}\coveragereport" -reporttypes:Html
	
	After running this command, an HTML file represents the generated report.

## The scenario when choosing **E2E testing to perform**

- Component with logic that depends on JS manipulation of the browser DOM.<br>
	When a component's functionality is dependent on JS and its manipulation of the DOM, verify both the JS and Blazor code together in an E2E test. This is the approach that the Blazor framework developers have taken with Blazor's browser rendering logic, which has tightly-coupled C# and JS code. The C# and JS code must work together to correctly render Blazor components in a browser.
- Component that depends on 3rd party component library with hard-to-mock dependencies.<br>
	When a component's functionality is dependent on a 3rd party component library that has hard-to-mock dependencies, such as JS interop, E2E testing might be the only option to test the component.

>Resources:<br>
https://docs.microsoft.com/en-us/aspnet/core/blazor/test?view=aspnetcore-3.1<br>
https://bunit.egilhansen.com/docs/getting-started/writing-razor-tests.html<br>
https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-code-coverage?tabs=windows

# Integration testing

## Repository testing

1. Inherit repository test classes from **"BaseRepositoryTests"** class.

		public class PositionRepositoryTests : BaseRepositoryTests

2. In arrange part add this line to get context:

		using var context = ConfigureContext();

3. Insert necessary data and save changes<br />
   **Example:**

		var patientModels = new List<PatientModel>
		{
		    new PatientModel
		    {
		        KeyId = 1001,
		        Id = "PAT-00001"
		    },
		    new PatientModel
		    {
		        KeyId = 1002,
		        Id = "110166-xxxx"
		    },
		};
		patientModels = patientModels.OrderBy(p => p.Id).ToList();
		await context.Patients.AddRangeAsync(patientModels);
		await context.SaveChangesAsync();

4. Run the act part<br />
   **Example:**

		var patients = await patientRepository.GetPatientsBasicInfoAsync();

5. Delete inserted data<br />
   **Example:**

		context.RemoveRange(patientModels);
		await context.SaveChangesAsync();


# Swagger

Swagger is the tool to generate API documentation, including a UI to explore and test operations, directly from your routes, controllers and models. 

To explore and test API methods follow this link https://localhost:5001/[production || search || meta-data]/swagger-api/index.html

Note: to **Try it out** API method that has attribute [Authorize] in code, you should use **Authorize** button at the top right of the *swagger-api/index.html* page. There you should enter 'Bearer [*access_token*]' in the text input.<br> 
To get the *access_token* you should follow these steps:
- Successfully Log in to our ScannerWeb application.
- Open "Developer tools" and copy the JSON Value of Key *oidc.user:[YourAppDomain]:ScannerWeb.Client*. For that press F12/Application tab/Store/Session Store/[YourAppDomain]/key oidc.user:[YourAppDomain]:ScannerWeb.Client.
- Parse this JSON Value and get *access_token* from there.

To validate and debug *access_token* and *id_token* navigate to this url https://jwt.io/ where you can paste these values and see Header, Payload and Verify Signature.

>Resources:<br>
https://github.com/domaindrivendev/Swashbuckle.AspNetCore


# Postman

Postman is a collaboration platform for API development.

To test API methods all you need is select the type of request (GET, POST etc), write request URL and press **Send** button.

Note: to test API method that has attribute [Authorize] in code, you should provide an authorization information (see Authorization tab of request).
In our case **TYPE** option should be set to *Bearer Token* and **Token** parameter should contain *access_token* (how to get *access_token* see above).


# Upgrade Bootstrap library

1. Download the Bootstrap Source from the Bootstrap site https://getbootstrap.com/docs/5.0/getting-started/download/ (pay attention to version).
2. Unzip archive.
3. Replace content of "DotNet\ScannerWeb\ScannerWeb.Client\Scss\Bootstrap\scss" by unziped "scss" folder.
4. Replace content of "DotNet\ScannerWeb\ScannerWeb.Client\wwwroot\lib\bootstrap\js" by unziped files "dist\js\bootstrap.min.js", "dist\js\bootstrap.min.js.map".

>Resources:<br>
https://getbootstrap.com/docs/5.0/customize/sass/
https://www.codeproject.com/Articles/5289921/Blazor-and-CSS-Frameworks
https://github.com/ShaunCurtis/Blazor.CSS


# Create and publish a NuGet package

## Create a class library project

1. In Visual Studio, choose **File > New > Project**, expand the Visual C# > .NET Standard node, select the "Class Library (A project for creating a class library that targets .NET Standard or .NET Core)" template, name the project, select **.NET Standard 2.1** as Target Framework and click Create.
2. Right-click on the resulting project file and select **Build** to make sure the project was created properly. The DLL is found within the Debug folder (or Release if you build that configuration instead).

## Configure package properties

1. Right-click the project in Solution Explorer, and choose **Properties** menu command, then select the **Package** tab.
2. Give your package a unique identifier and fill out any other desired properties (Package id, Package version, Authors, Company etc.). All of the properties here go into the *.nuspec* manifest that Visual Studio creates for the project.

## Run the pack command

1. Set the configuration to Release.
2. Right click the project in Solution Explorer and select the Pack command.
3. Visual Studio builds the project and creates the .nupkg file. Examine the **Output** window for details (similar to the following), which contains the path to the package file. Note also that the built assembly is in bin\Release\netstandard2.1 as befits the .NET Standard 2.1 target.

## (Optional) Generate package on build

You can configure Visual Studio to automatically generate the NuGet package when you build the project.
1. In Solution Explorer, right-click the project and choose **Properties**.
2. In the **Package tab**, select **Generate NuGet package on build**.

## Publish the package to Azure DevOps Artifacts repository

### Install nuget.exe

1. Visit https://www.nuget.org/downloads and select NuGet 3.3 or higher. The latest version is always recommended.
2. Each download is the nuget.exe file directly. Instruct your browser to save the file to a folder of your choice (e.g. C:\Tools).<br/>
The file is not an installer, you won't see anything if you run it directly from the browser.
3. Add the folder where you placed nuget.exe (C:\Tools) to your PATH environment variable to use the CLI tool from anywhere.

### Visual Studio setup

On the Tools menu, select **Options > NuGet Package Manager > Package Sources**. Select the green plus in the upper-right corner and enter the name and source URL below.

Name

	Test

Source

	https://devops.gldk.t-docnet.com/DefaultCollection/_packaging/Test/nuget/v3/index.json

Note: You need to do this on every machine that needs access to your packages. The current feed is named **Test**, so in case of using other feed name in Azure DevOps Artifacts repository the source url should be changed (just replace "Test" with needed feed name in the url). 

### Publish packages

Publish a package by providing the package path, an API Key (any string will do), and the feed URL

	nuget push -Source "Test" -ApiKey az <packagePath>

For example

	nuget push -Source "Test" -ApiKey az m:\Roadrunner\Packages\TDOC.Models\bin\Release\TDOC.Models.1.0.4.nupkg

### Restore packages

1. On the **Tools** menu, select **Options > NuGet Package Manager > Manage NuGet Packages for Solution...**.
2. Select Azure DevOps Artifacts repository in the **Package source** selector
3. Find and select a package you want to use on the **Browse** tab.
4. Mark all the needed projects in the current solution for which this package should be installed and click Install.



# Known issues and solving them

## Unable to configure HTTPS endpoint. No server certificate was specified, and the default developer certificate could not be found or is out of date.  

To generate a fresh developer certificate:
1. Open Run and open cmd.exe like administrator.
2. Run 'dotnet dev-certs https --clean' (to clear old bad or expired developer certificates).
3. Run 'dotnet dev-certs https --trust' (to generate and trust new developer certificate).

In case didn't work for you, please follow these steps:
1. Open Run and open mmc.exe.
2. Inside MMC from File menu click on Add/Remove Snap-in.
3. In the add/remove snap-in window, find certificates in available snap-ins and add it to the selected.
4. Pick Use account.
5. In the console root -> Certificates Current User -> Personal click on Certificates.
6. You will see the list of issued and installed certificates for the current user. 
**DON’T remove or change any certificates you don’t know**, only remove certificates related to self-sign localhost ASP.NET Core.
7. Open Run and open cmd.exe like administrator.
8. Run 'dotnet dev-certs https --trust' (to generate and trust new developer certificate).

>Resources:<br>
https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-5.0&tabs=visual-studio#troubleshoot-certificate-problems
https://itnext.io/installed-asp-net-16702767e7b3

## SSL Certificate problem: Unable to get local issuer certificate
Can happen on new developer environments. Change Cryptographic netowrk provider to 'Secure Channel'
1. In Visual Studio, go to Tools -> Options -> Source Control
2. Select Git Global Settings
3. Set 'Cryptographic network provider' to 'Secure Channel'

## Error on build: The command "webcompiler -f compilerFiles.conf -z disable" exited with code 1
This has been found to be caused by not being authorized to perform local script execution.
1. Contact serviceDesk and request to be allowed to run local script execution.
2. ServiceDesk should schedule this for your PC, which when applied will require you to restart the PC.
2. Run Visual Studio as Administrator and try again

## Cannot determine the organization name for this 'dev.azure.com' remote URL. Ensure the `credential.useHttpPath` configuration value is set...
Go to: Tools -> Options -> Source Control -> Git Global Settings. There change all 4 dropdowns which were still selected as "Unset":
- Prune remote branches during fetch - `False`
- Rebase local branch when pulling - `False`
- Cryptographic network provider - `OpenSSL`
- Credential helper - `GCM Core`

# TBD 

## Component library (?Syncfusion?)

## Authentication and Authorization

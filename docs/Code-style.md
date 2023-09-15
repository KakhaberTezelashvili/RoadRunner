# General Style Guide

## Naming rules

### Code

- Names of classes, methods, enumerations, public fields, public properties, public constants, namespaces: `PascalCase`.
  The only exception is for interop code where it should exactly match the name and value of the code you are calling via interop.
- Names of local variables, parameters: `camelCase`.
- **(TBD)** Names of private, protected, internal and protected internal fields and properties: 
  - by default use prefix `_` (e.g.: `_camelCase`);
  - with modifiers such as `const` use prefix `c_` (e.g.: `c_camelCase`);
  - with modifiers such as `readonly` use prefix `r_` (e.g.: `r_camelCase`).
- Naming convention is unaffected by modifiers such as `static`.
- For casing, a “word” is anything written without internal spaces, including acronyms (e.g.: `MyRpc` instead of ~~`MyRPC`~~, `MyLot` instead of ~~`MyLOT`~~).
- Names of interfaces start with `I` (e.g.: `IInterface`).

### REST API naming convention

#### Use nouns instead of verbs in endpoint paths
Don’t use verbs in our endpoint paths. Instead, use the nouns which represent the entity that the endpoint that we’re retrieving or manipulating as the pathname. 
HTTP request method already has the verb. Having verbs in our API endpoint paths isn’t useful and it makes it unnecessarily long since it doesn’t convey any new information. 
The chosen verbs could vary by the developer’s whim. For instance, some like get and some like retrieve, so it’s just better to let the HTTP GET verb tell us what an endpoint does.
- `GET` retrieves resources.
- `POST` submits new data to the server.
- `PUT` updates existing data.
- `DELETE` removes data.

**Example**:  
- `GET: /api/v1/customers` (gets list of customers).
- `GET: /api/v1/customers/1` (gets a customer with id 1).
- `POST: /api/v1/customers` (creates a customer).
- `PUT: /api/v1/customers/1` (updates a customer by id 1).
- `DELETE: /api/v1/customers/1` (removes a customer by id 1).

**Exceptions**: 
- For `GET` APIs that many data are needed to be passed to the server, `POST` verb is acceptable and instead of passing data through query parameters, use the body of the request.
- Use verb in URL in when action does not like CRUD operation (e.g.: `search`, `login` or `print`, `pack`, `return`).

#### Use logical nesting on endpoints
When designing endpoints, group those that contain associated information. If one object can contain another object, design the endpoint to reflect that. 

**Example**:
- `GET: /api/v1/units/1/contents` (gets list of contents related to unit with id 1).

#### Use hyphens (-) to improve readability
To make URLs easy for people to scan and interpret, use the hyphen (-) character to improve the readability of names in long path segments.

**Example**:
- `GET: /api/v1/texts/error-codes` instead of `/api/v1/texts/errorcodes`.

#### Query parameters
Use query parameters for:
- **Filtering**: To filter a collection when identifier field is not used and we want to filter data based on other fields of resource.<br>
  **Example**: `GET: /api/v1/machines?locationId={locationId }&type={sterilizers}`.
- **Paging**: To fetch portion of data.<br>
  **Example**: `GET: /api/v1/machines?pageNo=1&pageSize=10`
- **Sorting**: To sort data.<br>
  **Example**: `GET api/v1/customers?sortBy=firstName&orderBy=asc`.

#### camelCase for field names
Use `camelCase` naming for query parameters.<br>
**Example**: `GET: /api/v1/machines?pageNo=1&pageSize=10`

#### Route parameter constraint
Use [route constraints](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-6.0#route-constraints) to avoid wrong value binding.<br>
**Example**:
```
[Route("users/{id:int:min(1)}")]
public User GetUserById(int id) { }
```

#### Exceptions related to Roadrunner project
1.	The first resources match controller and the controller normally match the table name (e.g.: `api/v1/customers/{id}`, where [Customer = table name and controller]).
2.	Controller name must plural (e.g.: `CustomersController`).
3.	Max level of URL to get a resource is one level [Customer] (e.g.: `api/v1/customers?param1=111&param2=222`).
4.	For the `POST` and `PUT` always should be used `BODY` of request for arguments and `id` we should keep in URL (e.g.: `/api/v1/units/{id}/errors`).
5.	In case of adding the relationship between two resources like many to many relations: (e.g.: `/api/v1/primary-resource/id/secondary-resource`, `/api/v1/units/{id}/lots`).

### Method(s) convention
- Always define method names to be the same on each microservice levels: `ApiService`, `Endpoint`, `Service`, `Validator`, `Repository`. E.g.:
  ```
  // ApiService level.
  public interface IUnitReturnApiService
  {
    Task<UnitReturnDetailsDto> GetReturnDetailsAsync(int unitKeyId);
  }
  ```
  ```
  // Endpoint level.
  public class GetReturnDetailsAsync : EndpointBaseAsync
    .WithRequest<int>
    .WithActionResult<UnitReturnDetailsDto>
  {
    ...
    [SwaggerOperation(OperationId = "GetReturnDetailsAsync", Tags = new[] { EndpointsTags.UnitsReturn })]
    public override async Task<ActionResult<UnitReturnDetailsDto>> HandleAsync(int id, CancellationToken cancellationToken = default)
    {
      UnitReturnDetailsDto unitInfo = await _unitReturnService.GetReturnDetailsAsync(id);
      ...
    }
  }
  ```
  ```
  // Service level.
  public interface IUnitReturnService
  {
    Task<UnitReturnDetailsDto> GetReturnDetailsAsync(int unitKeyId);
  }
  ```
  ```
  // Validator level.
  public interface IUnitReturnValidator
  {
    Task<UnitModel> GetReturnDetailsValidateAsync(int userKeyId, UnitReturnArgs args);
  }
  ```
  ```
  // Repository level.
  public interface IUnitReturnRepository : IRepositoryBase<UnitModel>
  {
    Task<UnitReturnDetailsDto> GetReturnDetailsAsync(int unitKeyId);
  }
  ```

### Repository(s) convention
- Each repository represents an entity.
- Each repository interface should inherit `IRepositoryBase<>`.
- Each repository implementation should inherit `RepositoryBase<>`.
- Repository method naming convention is: `Add`, `Update`, `Remove`, `Find` and `Get`.
- For `Add`, `Update`, `Remove` use base methods and do not implement new ones.
- There are several other methods like `Count`, `Contain`, `FindById`, `GetAll` and `GetByPaging` in the base repository, use these methods instead of re-implementing them.
- Use the `Find` naming convention to return tracked entities by Entity Framework and use it in the service method for modifying data (update and remove). 
  Do not use this method to return data to the client.
- Use the `Get` naming convention to return untracked entities by Entity Framework and use it to return data to the client (use `AsNoTracking()` in query).
- Use `AddDataAsync`, `UpdateDataAsync` for custom implementation of adding or updating entity data. E.g.:
  ```
  public async Task<int> AddDataAsync(UnitModel data)
  {
    _context.Units.Add(data);
    await _context.SaveChangesAsync();
    return data.KeyId;
  }
  ```
- Avoid injecting repository into another repository.
- Avoid implementing business logic into repository methods. E.g.: a method should contain `for`, `if` ... for `Add`, `Update` and `Remove` and call other methods 
  (Exception: for `Get` method to construct `DTO`).

### Service(s) convention
- Service method naming convention is in general `Add`, `Update`, `Remove`, `Get` and `other verbs` that using in domain functionality.
- Use the `Find` prefix for the method name when returning tracked entities (by Entity Framework). Do not use this method to return data to the client.
- Use the `Get` prefix for the method name when returning untracked entities (by Entity Framework). Use this method to return data to the client.
  ```
  Task<UnitDataToPack> GetDataToPackAsync(UnitPackArgs args);
  Task<UnitPackDetailsDto> GetPackDetailsAsync(int unitKeyId);
  ```
- For the `FindBy` and `GetBy` methods always include information about filtering data in case there are.
  ```
  Task<ActionResult<EntityDetailsDto>> GetByKeyIdAsync(int id)
  Task<ActionResult<IList<EntityDetailsDto>>> GetByUserKeyIdOrFactoryKeyIdAsync(int? userId, int? factoryId)
  ```
- For the `FindWith` and `GetWith` methods always include information about extra data/entities to be returned back in case there are.
  ```
  Task<IList<UnitModel>> GetWithProductAndItemBySerialKeyIdsAsync(IEnumerable<int?> serialKeyIds);
  Task<MachineModel> GetWithMachineTypeByKeyIdValidateAsync(int keyId)
  ```
- Don't keep unnecessary information (words) in method names that already can be extracted from namespace or class name.<br>
  E.g.: in the next code we are not including word "Unit" in method names, but still keeps verb "Pack" to have indication what method will do:
  ```
  namespace ProductionService.Core.Services.Units.Pack;

  public interface IUnitPackService
  {
    Task<UnitPackDetailsDto> GetPackDetailsAsync(int unitKeyId);
    Task<IEnumerable<int>> PackAsync(int userKeyId, UnitPackArgs args);
  }
  ```

### Validator(s) convention
- Validators are for services or other validators. Based on service-method, for which it's written, validator method should always end with suffix `Validate`.
  ```
  // Service method.
  Task UpdatePatientAsync(int unitKeyId, int userKeyId, UnitPatientArgs args);
  // Validator method.
  Task<UnitModel> UpdatePatientValidateAsync(int unitKeyId, int userKeyId, UnitPatientArgs args);
  ```
  ```
  // Service method.
  Task<int> ReturnAsync(int userKeyId, UnitReturnArgs args);
  // Validator method.
  Task<UnitModel> ReturnValidateAsync(int userKeyId, UnitReturnArgs args);
  ```

### Files
- File names and directory names are `PascalCase` (e.g.: `MyFile.cs`).
- Where possible the file name should be the same as the name of the main class in the file (e.g.: `MyClass.cs`).
- In general, prefer one core class per file.
- Keep Data transfer object (DTO) inside `Dto` folder. There are could be two kind of files:
  - *Request DTO* should always end with suffix `Args`;
  - *Response DTO* should always end with suffix `Dto`;
  - Preference for naming is `BatchApproveArgs` and `BatchApproveDto` instead of `ApproveBatchArgs` and `ApproveBatchDto`, to have first word like indication what entity we will process.  
- Keep shared `const`(s) inside "feature" file that must be located in `Constants` folder.
- Keep shared `enum`(s) inside "feature" file that must be located in `Enumerations` folder.

#### For Blazor components
- Only add prefix `Td` for the own component that is wrapper over external component. E.g.: we have own component `TdButton` that is wrapper over `DxButton`.
- Always use quotation marks `""` for components attribute's value `IconUrl="@IconUrl"`.

## Error `enum` name convention

### Domain error code template
  ```
  /// <summary>
  /// Domain error codes shared between server and clients related to <feature> operations.
  /// - <SomeEntity1> errors have reserved numbers from 1 to 50.
  /// - <SomeEntity2> errors have reserved numbers from 51 to 100.
  /// - ...
  /// </summary>
  public enum Domain<Feature>ErrorCodes
  {
    /// <summary>
    /// Write here original error message that displaying for user on the client.
    /// </summary>
    <SomeEntity1>ErrorMessageIdentifierBasedOnSummary = 1,

    /// <summary>
    /// Write here original error message that displaying for user on the client.
    /// </summary>
    <SomeEntity1>ErrorMessageIdentifierBasedOnSummary = 2,

    ...

    /// <summary>
    /// Write here original error message that displaying for user on the client.
    /// </summary>
    <SomeEntity2>ErrorMessageIdentifierBasedOnSummary = 51,

    /// <summary>
    /// Write here original error message that displaying for user on the client.
    /// </summary>
    <SomeEntity2>ErrorMessageIdentifierBasedOnSummary = 52

    ...
  }
  ```

### E.g.: domain error codes
  ```
  /// <summary>
  /// Domain error codes shared between server and clients related to pack operations.
  /// - SerialNumber errors have reserved numbers from 1 to 50.
  /// - Unit errors have reserved numbers from 51 to 100.
  /// </summary>
  public enum DomainPackErrorCodes
  {
    /// <summary>
    /// The unit for pack associated with the serial number is missing a wash batch.
    /// </summary>
    SerialNumberUnitMissingWashBatch = 1,

    /// <summary>
    /// The unit for pack associated with the serial number does not have an approved wash batch.
    /// </summary>
    SerialNumberUnitWashBatchNotApproved = 2,

    /// <summary>
    /// Unit status for pack is not 'Returned'.
    /// </summary>
    UnitStatusNotReturned = 51,

    /// <summary>
    /// A unit for pack has already been packed based on this unit.
    /// </summary>
    UnitAlreadyPackedFromUnit = 52
  }
  ```

### Input argument error code template
  ```
  /// <summary>
  /// Input argument error codes shared between server and clients related to <entity>s.
  /// </summary>
  public enum InputArgument<Entity>ErrorCodes
  {
    /// <summary>
    /// Write here error message description.
    /// </summary>
    ErrorMessageIdentifierBasedOnSummary = 1,

    /// <summary>
    /// Write here error message description.
    /// </summary>
    ErrorMessageIdentifierBasedOnSummary = 2
  }
  ```

### E.g.: input argument error codes
  ```
  /// <summary>
  /// Input argument error codes shared between server and clients related to lots.
  /// </summary>
  public enum InputArgumentLotErrorCodes
  {
    /// <summary>
    /// Lot numbers does not match.
    /// </summary>
    NumbersNotMatch = 1,

    /// <summary>
    /// Lot items do not presence in unit content list.
    /// </summary>
    ItemsNotPresenceInUnitContentList = 2
  }
  ```

### Input argument error codes

## Class member ordering

### Group class members in the following order

#### Always
- Nested classes, enums, delegates and events.
- Static, const and readonly fields.
- Fields and properties.
- Constructors and finalizers.
- Methods.

#### For Blazor client(s)
- [Inject].
- [CascadingParameter].
- [Parameter].
- Static, const and readonly fields.
- Fields and properties.
- Lifecycle events:
  - SetParametersAsync;
  - OnInitialized/Async;
  - OnParametersSet/Async;
  - OnAfterRender/Async;
  - Dispose/Async.
- Methods.

### Within each group, elements should be in the following order
- Public.
- Internal.
- Protected internal.
- Protected.
- Private.

## Commenting conventions
- Place the comment on a separate line, not at the end of a line of code.
- Begin comment text with an uppercase letter.
- End comment text with a period.
- Insert one space between the comment delimiter (`//`) and the comment text, as shown in the following example.
  ```
  // The following declaration creates a query. It does not run
  // the query.
  ```
- Don't create formatted blocks of asterisks around comments.
- Ensure all public members have the necessary XML comments providing appropriate descriptions about their behavior.
- For the case when we have interface and implementation of some feature,  then provide all necessary descriptions about methods inside interface file and
  add `/// <inheritdoc />` or `/// <inheritdoc cref="ISomeService" />` for all realize methods inside implementation file.
- All constructors should always have defined comments `<param>` in `ApiService`, `Endpoint`, `Service`, `Validator`, `Repository` for all input parameters. 
  Description should be taken from `<summary>` of injected interfaces.
- Always define comments for feature method to be the same on each microservice levels: `ApiService`, `Endpoint`, `Service`, `Validator`, `Repository`. E.g.:

### Templates for comments
- Api services (*E.g.:* used in MasterDataService.Client.Services).
  ```
  /// <summary>
  /// API service provides methods to retrieve/handle <entity>s.
  /// </summary>
  public interface IEntityApiService
  ```
- Api controllers (*E.g.:* used in MasterDataService.API.Controllers).
  ```
  /// <summary>
  /// EF controller provides methods to retrieve/handle <entity>s.
  /// </summary>
  public class EntitiesController : ApiControllerBase
  ```
  ```
  // GET: api/v1/<entity>s/1001
  /// <summary>
  /// Retrieves <entity> by key identifier.
  /// </summary>
  /// <param name="id"><Entity> key identifier.</param>
  /// <returns>
  /// Action result indicating the result of the operation; if the operation was successful,
  /// <entity> is returned as part of the response.
  /// </returns>
  /// <response code="400">
  /// Bad request - check your input arguments.
  /// </response>
  [HttpGet("{id:int}")]
  ...
  public async Task<ActionResult<EntityDetailsDto>> GetByKeyIdAsync(int id)
  ```
  ```
  // GET: api/v1/<entity>s?userId=1001&factoryId=100
  /// <summary>
  /// Retrieves <entity>s by user or factory key identifier.
  /// </summary>
  /// <param name="userId">User key identifier.</param>
  /// <param name="factoryId">Factory key identifier.</param>
  /// <returns>
  /// Action result indicating the result of the operation; if the operation was successful, 
  /// collection of <entity>s is returned as part of the response.
  /// </returns>
  /// <response code="400">
  /// Bad request - check your input arguments.
  /// </response>
  ...
  public async Task<ActionResult<IList<EntityDetailsDto>>> GetByUserKeyIdOrFactoryKeyIdAsync(int? userId, int? factoryId)
  ...
  ```
- Services (*E.g.:* used in MasterDataService.Core.Services).
  ```
  /// <summary>
  /// Service provides methods to retrieve/handle <entity>s.
  /// </summary>
  public interface IEntityService
  ```
  ```
  /// <summary>
  /// Retrieves <entity> by key identifier.
  /// </summary>
  /// <param name="keyId"><Entity> key identifier.</param>
  /// <returns>Entity.</returns>
  Task<EntityModel> GetByKeyIdAsync(int keyId);
  ```
- Validators (*E.g.:* used in MasterDataService.Core.Services).
  ```
  /// <summary>
  /// Validator provides methods to validate <entity>s.
  /// </summary>
  public interface IEntityValidator
  ```
  ```
  /// <summary>
  /// Validates retrieving <entity> by key identifier.
  /// </summary>
  /// <param name="keyId"><Entity> key identifier.</param>
  /// <returns>If the validation passes, <entity> will be returned.</returns>
  Task<EntityModel> GetByKeyIdValidateAsync(int keyId);
  ```
  ```
  /// <summary>
  /// Validates packing new unit(s).
  /// </summary>
  /// <param name="args">Unit pack arguments.</param>
  /// <returns>If the validation passes, unit data to pack will be returned.</returns>
  Task<UnitDataToPack> PackValidateAsync(UnitPackArgs args);
  ```
- Repositories (*E.g.:* used in MasterDataService.Infrastructure.Repositories).
  ```
  /// <summary>
  /// Repository provides methods to retrieve/handle <entity>s.
  /// </summary>
  public interface IEntityRepository : IRepositoryBase<EntityModel>
  ```
  ```
  /// <summary>
  /// Initializes a new instance of the <see cref="EntityRepository" /> class.
  /// </summary>
  /// <param name="context">EF database context.</param>
  public EntityRepository(TDocEFDbContext context) : base(context)
  {
  }
  ```
- Constructors.
  ```
  /// <summary>
  /// Initializes a new instance of the <see cref="<ClassName>" /> class.
  /// </summary>
  ```
- Properties.
  ```
  /// <summary>
  /// Customer key identifier.
  /// </summary>
  public int CustomerKeyId { get; set; }
  ```
  ```
  /// <summary>
  /// Collection of unit key identifiers.
  /// </summary>
  public IList<int> UnitKeyIds { get; set; }
  ```
- Others:
  - Only add `/// <returns>...</returns>` comment for the method when it's returning some object or value. 
  - Don't add `/// <returns>...</returns>` comment for the method when it's returning just `System.Threading.Tasks.Task`. E.g.:
    ```
    /// <summary>
    /// Updates the unit with the specified error.
    /// </summary>
    /// <param name="args">Unit error arguments.</param>
    Task UpdateErrorAsync(UnitErrorArgs args);
    ```

## Spaces and empty lines
- We use four spaces of indentation (no tabs).
- Avoid more than one empty line at any time. E.g.: do not have two blank lines between members of a type.
- Avoid spurious free spaces. E.g.: avoid `if (someVar == 0)...`, where the dots mark the spurious free spaces.
  Consider enabling "View White Space (Ctrl+R, Ctrl+W)" or "Edit -> Advanced -> View White Space" if using Visual Studio to aid detection.
- No line break before opening brace.
- No line break between closing brace and `else`.
- Space after `if`/`for`/`while` etc., and after commas.
- No space after an opening parenthesis or before a closing parenthesis.
- No space between a unary operator and its operand. One space between the operator and each operand of all other operators.

## Each curly-braces in own new line
- We use [Allman style](http://en.wikipedia.org/wiki/Indent_style#Allman_style) braces, where each brace begins on a new line.
- Never use single-line form (e.g.: `if (source == null) throw new ArgumentNullException("source");`).
- Using braces is always accepted, and required if any block of an `if`/`else if`/.../`else` compound statement uses braces or if a single statement body spans multiple lines.
- Braces may be omitted only if the body of *every* block associated with an `if`/`else if`/.../`else` compound statement is placed on a single line.
  In other words, a single line statement block can go without braces but the block must be properly indented on its own line and must not be nested in other statement blocks that use braces.

## Namespace imports (using)
- Namespace imports should be specified at the top of the file, *outside* of `namespace` declarations, and should be sorted alphabetically.

## Visibility modifier
- We always specify the visibility, even if it's the default (e.g.: `private string _foo` not `string _foo`).
- Visibility should be the first modifier (e.g.: `public abstract` not `abstract public`).

## Language keywords
- We use language keywords instead of BCL types (e.g.: `int, string, float` instead of `Int32, String, Single`, etc) for both type references as well as method calls
  (e.g.: `int.Parse` instead of `Int32.Parse`). See issue [#13976](https://github.com/dotnet/runtime/issues/13976).

## Non-ASCII characters
- When including non-ASCII characters in the source code use Unicode escape sequences (`\uXXXX`) instead of literal characters.
  Literal non-ASCII characters occasionally get garbled by a tool or editor.

## new operator
- Use one of the concise forms of object instantiation, as shown in the following declarations.
  ```
  var instance1 = new ExampleClass();
  ```
  ```
  ExampleClass instance2 = new();
  ```
- Use object initializers to simplify object creation, as shown in the following example.
  ```
  var instance3 = new ExampleClass { 
    Name = "Desktop", 
    ID = 37414,
    Location = "Redmond", 
    Age = 2.3 
  };
  ```
## namespace
Always use short semantic `namespace X.Y.Z;`. The semantics are that using the `namespace X.Y.Z;` form is equivalent to writing `namespace X.Y.Z { ... }` 
where the remainder of the file following the file-scoped namespace is in the `...` section of a standard namespace declaration.

##  && and || operators
- To avoid exceptions and increase performance by skipping unnecessary comparisons, use `&&` instead of `&` and `||` instead of `|` when you perform comparisons.

## this.
- We avoid `this.` unless absolutely necessary.

## nameof
- We use ```nameof(...)``` instead of ```"..."``` whenever possible and relevant.

## The var keyword
- Use of `var` is encouraged if it aids readability by avoiding type names that are noisy, obvious, or unimportant.
- We only use `var` when the type is explicitly named on the right-hand side, typically due to either `new` or an explicit cast.
- Encouraged:
  - When the type is obvious (e.g.: `var apple = new Apple();`, or `var request = Factory.Create<HttpRequest>();`).
  - For transient variables that are only passed directly to other methods (e.g.: `var item = GetItem(); ProcessItem(item);`).
- Discouraged:
  - When working with basic types (e.g.: `var success = true;`).
  - When working with compiler-resolved built-in numeric types (e.g.: `var number = 12 * ReturnsFloat();`).
  - When users would clearly benefit from knowing the type (e.g.: `var listOfItems = GetList();`).

## Expression-Bodied Members (=>)
Expression-bodied members provide a minimal and concise syntax to define properties and methods. It helps to eliminate boilerplate code and helps writing code that is more readable. 
The expression-bodied syntax can be used when a member's body consists only of one expression. 
It uses the `=>`(fat arrow) operator to define the body of the method or property and allows getting rid of curly braces `{}` and the `return` keyword. 
- Expression-bodied methods:
  ```
  public static bool IsEven(int number) => number % 2 == 0;
  private int GetRectangleArea(int length, int breadth) => length * breadth;
  ```
- Expression-bodied void methods:
  ```
  void PrintName(string name) => Console.WriteLine($"The name is {name}");
  ```
- Expression-bodied read-only properties:
  ```
  public int Name => "Geeks For Geeks";
  ```
- Expression-bodied non read-only properties:
  ```
  private string name;  
  public string Name
  {
    get => name;
    set => name = value;
  }
  ```

For the more details read [this](https://www.geeksforgeeks.org/expression-bodied-members-in-c-sharp/).

## IEnumerable vs IList vs IReadOnlyList
- For inputs use the most restrictive collection type possible (e.g.: `IReadOnlyCollection` / `IReadOnlyList` / `IEnumerable` 
  as inputs to methods when the inputs should be immutable).
- For outputs, if passing ownership of the returned container to the owner, prefer `IList` over `IEnumerable`.
  If not transferring ownership, prefer the most restrictive option.

## Property styles
- For single line read-only properties, prefer expression body properties (`=>`) when possible.
- For everything else, use the older `{ get; set; }` syntax.

## Lambdas vs named methods
- If a lambda is non-trivial (e.g.: more than a couple of statements, excluding declarations), or is reused in multiple places, it should probably be a named method.

## LINQ
- Prefer member extension methods ([method syntax](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/query-syntax-and-method-syntax-in-linq)) 
  over SQL-style LINQ keywords (query syntax). E.g.: prefer `myList.Where(x)` to `myList where x`.

## Use record for DTO(s) and Args
- A DTO (Data Transport Object) is a classic case where immutability is a desired feature. Thats why always use `record` for DTO(s) and Args.
- Always use [Init Only Setters](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/init). 
  An init-only setter assigns a value to the property or the indexer element only during object construction.
  ```
  public record Person
  {
    public string FirstName { get; init; }
    public string LastName { get; init; }
  }
  ```

## Use of tuple as a return type
- In general, prefer a named class type over `Tuple<>`, particularly when returning complex types.

## Calling delegates
- When calling a delegate, use `Invoke()` and use the null conditional operator (e.g.: `SomeDelegate?.Invoke()`).
  This clearly marks the call at the callsite as ‘a delegate that is being called’. The null check is concise and robust against threading race conditions.

## Tools (TBD)
An [EditorConfig](https://editorconfig.org "EditorConfig homepage") file (`.editorconfig`) has been provided at the root of the runtime repository, 
enabling C# auto-formatting conforming to the above guidelines.

We also use the [.NET Codeformatter Tool](https://github.com/dotnet/codeformatter) to ensure the code base maintains a consistent style over time, 
the tool automatically fixes the code base to conform to the guidelines outlined above.

>Resources:<br>
>https://google.github.io/styleguide/csharp-style.html<br>
>https://github.com/dotnet/runtime/blob/main/docs/coding-guidelines/coding-style.md<br>
>https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions<br>
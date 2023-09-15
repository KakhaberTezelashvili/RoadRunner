# OpenSSL 

To setup OpenSSL on your local machine follow next steps:

1. Download "Win64 OpenSSL v1.1.1k" from https://slproweb.com/products/Win32OpenSSL.html.
2. Setup OpenSSL by running just downloaded installer (set destination location to be C:\Tools\OpenSSL).
3. Press Windows + R keys together to open Run dialog. 
4. Enter *sysdm.cpl* in the Run dialog box and hit Enter to open System Properties. 
5. Go to "Advanced" tab and click on "Environment variables".
6. Update Sytem variables "Path" to include C:\Tools\OpenSSL\bin.

To use OpenSSL:

1. Open Command Prompt (cmd.exe) like administrator.
2. Enter *openssl* and hit Enter.

>Resources:<br>
https://tecadmin.net/install-openssl-on-windows/<br>
https://slproweb.com/products/Win32OpenSSL.html


# Generating Personal Information Exchange file (*.pfx) 

To generate localhost.pfx file follow next steps:

1. Open Command Prompt (cmd.exe) like administrator.
2. Change path to be *cd M:\Roadrunner*.
3. Run command to generate localhost.pfx file with password *qwer1234*:
        
        dotnet dev-certs https -ep ssl\localhost.pfx -p qwer1234
        dotnet dev-certs https --trust

>**Notes:**<br>
>As soon as you decide:
>- to keep "localhost.pfx" file in different location than M:\Roadrunner\ssl 
>- to rename "localhost.pfx" file
>- to use different password
>
>then you should: 
> 1. update *M:\Roadrunner\docker-compose.yml*
> 2. generate new file M:\Roadrunner\ssl\certs\localhost.crt 
> 3. generate new file M:\Roadrunner\ssl\private\localhost.key


# Generating certificate files (*.crt and *.key) based on *.pfx

Extracting certificate and private key information from a Personal Information Exchange (.pfx) file using OpenSSL:

1. Open Command Prompt (cmd.exe) like administrator.
2. Change path to be *cd M:\Roadrunner*.
3. Run command *OpenSSL*.
4. Run the following OpenSSL command:
        
        pkcs12 -in ssl\localhost.pfx -out ssl\localhost.txt -nodes

5. Enter the password for the .pfx file (we set previously it to *qwer1234*).

Creating your localhost.crt file: 

1. Create a new empty file M:\Roadrunner\ssl\certs\localhost.crt.
2. Open the newly generated *ssl\localhost.txt* file above.
3. Copy the section starting from and including *-----BEGIN CERTIFICATE-----* to *-----END CERTIFICATE-----*.
4. Paste just copied text into M:\Roadrunner\ssl\certs\localhost.crt and save it.

Creating your localhost.key file: 

1. Create a new empty file M:\Roadrunner\ssl\private\localhost.key.
2. Open the newly generated *ssl\localhost.txt* file above.
3. Copy the section starting from and including *-----BEGIN PRIVATE KEY-----* to *-----END PRIVATE KEY-----*.
4. Paste just copied text into M:\Roadrunner\ssl\private\localhost.key and save it.

>**Notes:**<br>
>As soon as you decide:
>- to keep "localhost.crt" file in different location than M:\Roadrunner\ssl\certs\ 
>- to keep "localhost.key" file in different location than M:\Roadrunner\ssl\private\ 
>- to rename "localhost.crt", "localhost.key" file
>
>then you should also update *M:\Roadrunner\ScannerClient\ScannerClient.WebApp\Dockerfile*.

>Resources:<br>
https://helpcenter.gsx.com/hc/en-us/articles/115015887447-Extracting-Certificate-crt-and-PrivateKey-key-from-a-Certificate-pfx-File
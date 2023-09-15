FROM mcr.microsoft.com/mssql/server:2019-latest

USER root

# Copy initialization scripts.
COPY CreateScripts/setup_001.sql setup.sql
COPY Docker/import-data.sh import-data.sh
COPY Docker/entrypoint.sh entrypoint.sh

# Grant permissions for the import-data script to be executable.
RUN chmod +x import-data.sh

# Set environment variables, not to have to write them with docker run command.
# Note: make sure that your password matches what is in the import-data script. 
ENV SA_PASSWORD rOadrUnner1234
ENV ACCEPT_EULA Y

# Run Microsoft Sql Server and initialization script (at the same time).
# Note: If you want to start MsSQL only (without initialization script) you can comment bellow line out, CMD entry from base image will be taken.
CMD /bin/bash ./entrypoint.sh
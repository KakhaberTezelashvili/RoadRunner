# 50 seconds is enough to initialization of SQL Server. It can be increased if initialization is slow.
for i in {1..50};
do
    # Run the setup script to create the DB and the schema in the DB.
    # Note: make sure that your password matches what is in the Dockerfile.
    /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P rOadrUnner1234 -d master -i setup.sql
    if [ $? -eq 0 ]
    then
        echo "setup.sql completed"
        break
    else
        echo "not ready yet..."
        # Check again in every 1 second.
        sleep 1
    fi
done
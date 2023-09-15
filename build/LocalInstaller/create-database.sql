USE [master]
GO
RESTORE DATABASE [RoadrunnerTest] FROM  DISK = '/usr/src/app/DBBACKUP.BAK'
WITH REPLACE,  MOVE 'TDOC' 
TO '/usr/src/app/RoadrunnerTest.mdf',  MOVE 'TDOC_log' 
TO '/usr/src/app/RoadrunnerTest_log.ldf',  NOUNLOAD,  STATS = 5
GO
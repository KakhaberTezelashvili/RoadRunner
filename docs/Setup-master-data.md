# Setup workflows
 
Roadrunner works only with ptReturnWF, ptWashPreBatchWF, ptSteriPreBatchWF, ptPackWF, ptOutWF, ptWashPostBatchWF, ptSteriPostBatchWF locations that is why we need to add them.
To add these locations:
 
1. Run LDesign application.
2. Run the latest TDAdmin v17.
3. Login with SYS user.
4. Go to System > Setup > Locations.
5. Create new locations for last 7 processes (Pack units, Return units, Create wash batch, Create sterilizer batch, Dispatch to customer, Handle wash batch, Handle sterilizer batch).
   The name of the locations needs to be like the name of the appropriate process.
6. For each location you must set the position – Multi (go to tab Position and check Multi position).

# Setup sterilizers at 'Create sterilizer batch' location (ptSteriPreBatchWF)

## 1. Setup sterilizers using SQL-script

1. Create 4 new machine models of Sterilizer type: Model ST-01, Model ST-02, Model ST-03 and Model ST-04.

        -- Creating machine models
        INSERT INTO TMACHINT 
        (MCTYPKEYID, MCTYPNAME, MCTYPTEXT, MCTYPTYPE, MCTYPSTANDARDERROR, 
         MCTYPPRESSURETYPE, MCTYPTEMPTYPE, MCTYPTEMPDECIMALS, MCTYPMINPRES, MCTYPMAXPRES, MCTYPMINTEMP, 
         MCTYPMAXTEMP, MCTYPPLACEMENTCOUNT, MCTYPPLACEMENTMODE, MCTYPCBIUSAGEMODE, MCTYPLOGBATCHREGMODE, 
         MCTYPLOGBATCHREGERROR, MCTYPABSOLUTEPRESSURE, MCTYPUSERECIPE, MCTYPUSEMAP)
        VALUES (101, 'Model ST-01', 'Sterilizer model 1', 0, 0, 
                20529, 21553, 1, 0, 3000000, 0, 20000, 0, 0, 0, 0, 0, 'F', 'F', 'F'),
	           (102, 'Model ST-02', 'Sterilizer model 2', 0, 0, 
                20529, 21553, 1, 0, 3000000, 0, 20000, 0, 0, 0, 0, 0, 'F', 'F', 'F'),
	           (103, 'Model ST-03', 'Sterilizer model 3', 0, 0, 
                20529, 21553, 1, 0, 3000000, 0, 20000, 0, 0, 0, 0, 0, 'F', 'F', 'F'),
	           (104, 'Model ST-04', 'Sterilizer model 4', 0, 0, 
                20529, 21553, 1, 0, 3000000, 0, 20000, 0, 0, 0, 0, 0, 'F', 'F', 'F');

2. Create 4 new sterilizers based on the created machine models: ST-01, ST-02, ST-03 and ST-04.

        -- Creating machines
        INSERT INTO TMACHINE
        (MACHKEYID, MACHMCTYPKEYID, MACHNAME, MACHTEXT, MACHCYCLECOUNT, MACHMINUTESON, MACHMINUTESRUN, MACHREFNUM,
         MACHLASTCYCLECOUNT, MACHLASTMINUTESON, MACHLASTMINUTESRUN, MACHCHARGENUMMODE, MACHWARNEMPTYBATCH, 
         MACHACTUALPROGRAMCHECK, MACHPRINTLABELEND, MACHPRINTLABELAPPROVE, MACHPRINTLABELDISAPPR, MACHPRINTLISTINIT, 
         MACHPRINTLISTSTART, MACHPRINTLISTEND, MACHPRINTLISTAPPROVE, MACHPRINTLISTDISAPPR, MACHPRINTLABELSTICKERS,
         MACHPRESCAN, MACHLOGBATCHCONTENT, MACHALLOWLOG, MACHALLOWBATCH, MACHPRINTLABELINIT, MACHPRINTLABELSTART)
        VALUES (101, 101, 'ST-01', 'Sterilizer 1', 0, 0, 0, 101,
                0, 0, 0, 0, 1, 0, 'T', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 0, 'F', 'T', 'F', 'F'),
               (102, 102, 'ST-02', 'Sterilizer 2', 0, 0, 0, 102,
                0, 0, 0, 0, 1, 0, 'T', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 0, 'F', 'T', 'F', 'F'),
               (103, 103, 'ST-03', 'Sterilizer 3', 0, 0, 0, 103,
                0, 0, 0, 0, 1, 0, 'T', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 0, 'F', 'T', 'F', 'F'),
               (104, 104, 'ST-04', 'Sterilizer 4', 0, 0, 0, 104,
                0, 0, 0, 0, 1, 0, 'T', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 0, 'F', 'T', 'F', 'F');

3. Add the created sterilizers into TDOC.ini using TDIniEdit.
- Launch TDIniEdit
- Select and collapse 'Machines' branch on the left tree view, click 'Action' -> 'New' to create a new machine, fill-in the next data and click 'Ok' button for applying changes:<br>
    - **Name:** ST-01<br>
      (for the rest of machines: ST-02, ST-03, ST-04)
    - **No:** 101<br>
      (for the rest of machines: 102, 103, 104). For making this field editable press and hold Ctrl button and click on it
    - **Attached to:** [your PC name] (A)
    - **Port:** SCK 101<br>
      (for the rest of machines: 102, 103, 104)
    - **Type:** Sterilizer (S)
    - **Control system:** No communication (MANUAL)
- Repeat the previous step for the rest 3 machines.
- Save your changes to TDOC.ini-file ('File' -> 'Save') and close the application.

4. Add Positions for the new sterilizers.
- Launch TDAdmin - the next information dialog will be displayed: "Position for Machine ST-01 not found on Station A..."
- Click 'Ok' button - Positions window is automatically opened and the next confirmation dialoge is displayed: "Positions/Locations are not current. Update them automatically?".
- Click 'Ok' button, then click 'Auto create' button and select any displayed location for each of the sterilizers.
- Close TDAdmin.

5. Update new sterilizers positions with 'Create sterilizer batch' location.

        -- Updating positions with 'Create sterilizer batch' location (such location types are not allowed to attach to sterilizers in TDAdmin yet)
        UPDATE TPOSLOCA
        SET PLOLOCAKEYID = (SELECT TOP 1 LOCAKEYID FROM TLOCATIO WHERE LOCAPROCESS = 31)
        WHERE PLOPOSKEYID IN (SELECT POSKEYID FROM TPOSIT WHERE POSTHING = 2 AND POSNAME IN ('ST-01', 'ST-02', 'ST-03', 'ST-04'))

## 2. Setup sterilizers in TDAdmin.

... to be continued ...


# Setup programs for sterilizers

## 1. Setup programs using SQL-script

    -- Creating sterilizers programs
    INSERT INTO TPROGRAM
    (PROGKEYID, PROGMCTYPKEYID, PROGPROGRAM, PROGNAME, PROGTYPE, PROGSCANNUM, PROGDURATION, PROGAPPROVAL, PROGCHECKINITIATED)
    VALUES (101, 101, 'P1', 'Program 1 (ST-01)', 0, 1, 1800, 0, 0),
           (102, 101, 'P2', 'Program 2 (ST-01)', 0, 2, 1800, 0, 0),
           (103, 101, 'P3', 'Program 3 (ST-01)', 0, 3, 1800, 0, 0),
           (104, 101, 'P4', 'Program 4 (ST-01)', 0, 4, 1800, 0, 0),
           (105, 102, 'P1', 'Program 1 (ST-02)', 0, 1, 1800, 0, 0),
           (106, 102, 'P2', 'Program 2 (ST-02)', 0, 2, 1800, 0, 0),
           (107, 102, 'P3', 'Program 3 (ST-02)', 0, 3, 1800, 0, 0),
           (108, 103, 'P1', 'Program 1 (ST-03)', 0, 1, 1800, 0, 0),
           (109, 103, 'P2', 'Program 2 (ST-03)', 0, 2, 1800, 0, 0),
           (110, 104, 'P1', 'Program 1 (ST-04)', 0, 1, 1800, 0, 0);

## 2. Setup programs in TDAdmin.

1. Launch TDAdmin.
2. Open 'Machines' window (System -> Machines -> Machines).
3. Locate the sterilizer machine you have created before (ST-01) and click 'Model' caption - you will be redirected to the model of the sterilizer machine is based on (Model ST-01).
4. Click 'Programs' toolbar button - 'Programs' window will be opened.
5. Click 'New' button to create a new program associated with the selected machine model and fill-in the next obligatory fields:<br>
  - **Program:** P1<br>
    (for the rest of programs of the same machine model: P2, P3, P4 etc.)
  - **Type:** Normal
  - **Internal no.:** 1<br>
    (for the rest of programs of the same machine model: 2, 3, 4 etc.)
  - **Cycle duration (m):** 30<br>
  Then click 'Save' button, specify the program name (Program 1 (ST-01)) and save the change.
6. Repeat step 5 as many times as you need to have a different programs for the specified machine model.
7. Repeat steps 3-6 to add new programs to the rest of sterilizers.


# Setup washers at 'Create washer batch' location (ptWashPreBatchWF)

## 1. Setup washers using SQL-script

1. Create 4 new machine models of washer type: Model WA-01, Model WA-02, Model WA-03 and Model WA-04.

        -- Creating machine models
        INSERT INTO TMACHINT 
        (MCTYPKEYID, MCTYPNAME, MCTYPTEXT, MCTYPTYPE, MCTYPSTANDARDERROR, 
         MCTYPPRESSURETYPE, MCTYPTEMPTYPE, MCTYPTEMPDECIMALS, MCTYPMINPRES, MCTYPMAXPRES, MCTYPMINTEMP, 
         MCTYPMAXTEMP, MCTYPPLACEMENTCOUNT, MCTYPPLACEMENTMODE, MCTYPCBIUSAGEMODE, MCTYPLOGBATCHREGMODE, 
         MCTYPLOGBATCHREGERROR, MCTYPABSOLUTEPRESSURE, MCTYPUSERECIPE, MCTYPUSEMAP)
        VALUES (201, 'Model WA-01', 'Washer model 1', 1, 0, 
                20529, 21553, 1, 0, 3000000, 0, 20000, 0, 0, 0, 0, 0, 'F', 'F', 'F'),
	           (202, 'Model WA-02', 'Washer model 2', 1, 0, 
                20529, 21553, 1, 0, 3000000, 0, 20000, 0, 0, 0, 0, 0, 'F', 'F', 'F'),
	           (203, 'Model WA-03', 'Washer model 3', 1, 0, 
                20529, 21553, 1, 0, 3000000, 0, 20000, 0, 0, 0, 0, 0, 'F', 'F', 'F'),
	           (204, 'Model WA-04', 'Washer model 4', 1, 0, 
                20529, 21553, 1, 0, 3000000, 0, 20000, 0, 0, 0, 0, 0, 'F', 'F', 'F');

2. Create 4 new washers based on the created machine models: ST-01, ST-02, ST-03 and ST-04.

        -- Creating machines
        INSERT INTO TMACHINE
        (MACHKEYID, MACHMCTYPKEYID, MACHNAME, MACHTEXT, MACHCYCLECOUNT, MACHMINUTESON, MACHMINUTESRUN, MACHREFNUM,
         MACHLASTCYCLECOUNT, MACHLASTMINUTESON, MACHLASTMINUTESRUN, MACHCHARGENUMMODE, MACHWARNEMPTYBATCH, 
         MACHACTUALPROGRAMCHECK, MACHPRINTLABELEND, MACHPRINTLABELAPPROVE, MACHPRINTLABELDISAPPR, MACHPRINTLISTINIT, 
         MACHPRINTLISTSTART, MACHPRINTLISTEND, MACHPRINTLISTAPPROVE, MACHPRINTLISTDISAPPR, MACHPRINTLABELSTICKERS,
         MACHPRESCAN, MACHLOGBATCHCONTENT, MACHALLOWLOG, MACHALLOWBATCH, MACHPRINTLABELINIT, MACHPRINTLABELSTART)
        VALUES (201, 201, 'WA-01', 'Washer 1', 0, 0, 0, 201,
                0, 0, 0, 0, 1, 0, 'T', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 0, 'F', 'T', 'F', 'F'),
               (202, 202, 'WA-02', 'Washer 2', 0, 0, 0, 202,
                0, 0, 0, 0, 1, 0, 'T', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 0, 'F', 'T', 'F', 'F'),
               (203, 203, 'WA-03', 'Washer 3', 0, 0, 0, 203,
                0, 0, 0, 0, 1, 0, 'T', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 0, 'F', 'T', 'F', 'F'),
               (204, 204, 'WA-04', 'Washer 4', 0, 0, 0, 204,
                0, 0, 0, 0, 1, 0, 'T', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 0, 'F', 'T', 'F', 'F');

3. Add the created washers into TDOC.ini using TDIniEdit.
- Launch TDIniEdit
- Select and collapse 'Machines' branch on the left tree view, click 'Action' -> 'New' to create a new machine, fill-in the next data and click 'Ok' button for applying changes:<br>
    - **Name:** WA-01<br>
      (for the rest of machines: WA-02, WA-03, WA-04)
    - **No:** 201<br>
      (for the rest of machines: 202, 203, 204). For making this field editable press and hold Ctrl button and click on it
    - **Attached to:** [your PC name] (A)
    - **Port:** SCK 201<br>
      (for the rest of machines: 202, 203, 204)
    - **Type:** Washer (W)
    - **Control system:** No communication (MANUAL)
- Repeat the previous step for the rest 3 machines.
- Save your changes to TDOC.ini-file ('File' -> 'Save') and close the application.

4. Add Positions for the new washers.
- Launch TDAdmin - the next information dialog will be displayed: "Position for Machine WA-01 not found on Station A..."
- Click 'Ok' button - Positions window is automatically opened and the next confirmation dialoge is displayed: "Positions/Locations are not current. Update them automatically?".
- Click 'Ok' button, then click 'Auto create' button and select any displayed location for each of the washers.
- Close TDAdmin.

5. Update new washers positions with 'Create washer batch' location.

        -- Updating positions with 'Create washer batch' location (such location types are not allowed to attach to washers in TDAdmin yet)
        UPDATE TPOSLOCA
        SET PLOLOCAKEYID = (SELECT TOP 1 LOCAKEYID FROM TLOCATIO WHERE LOCAPROCESS = 30)
        WHERE PLOPOSKEYID IN (SELECT POSKEYID FROM TPOSIT WHERE POSTHING = 2 AND POSNAME IN ('WA-01', 'WA-02', 'WA-03', 'WA-04'))

## 2. Setup washers in TDAdmin.

... to be continued ...

# Setup programs for washers

## 1. Setup programs using SQL-script

    -- Creating washers programs
    INSERT INTO TPROGRAM
    (PROGKEYID, PROGMCTYPKEYID, PROGPROGRAM, PROGNAME, PROGTYPE, PROGSCANNUM, PROGDURATION, PROGAPPROVAL, PROGCHECKINITIATED)
    VALUES (201, 201, 'P1', 'Program 1 (WA-01)', 0, 1, 1800, 0, 0),
           (202, 201, 'P2', 'Program 2 (WA-01)', 0, 2, 1800, 0, 0),
           (203, 201, 'P3', 'Program 3 (WA-01)', 0, 3, 1800, 0, 0),
           (204, 201, 'P4', 'Program 4 (WA-01)', 0, 4, 1800, 0, 0),
           (205, 202, 'P1', 'Program 1 (WA-02)', 0, 1, 1800, 0, 0),
           (206, 202, 'P2', 'Program 2 (WA-02)', 0, 2, 1800, 0, 0),
           (207, 202, 'P3', 'Program 3 (WA-02)', 0, 3, 1800, 0, 0),
           (208, 203, 'P1', 'Program 1 (WA-03)', 0, 1, 1800, 0, 0),
           (209, 203, 'P2', 'Program 2 (WA-03)', 0, 2, 1800, 0, 0),
           (210, 204, 'P1', 'Program 1 (WA-04)', 0, 1, 1800, 0, 0);

## 2. Setup programs in TDAdmin.

1. Launch TDAdmin.
2. Open 'Machines' window (System -> Machines -> Machines).
3. Locate the washer machine you have created before (WA-01) and click 'Model' caption - you will be redirected to the model of the washer machine is based on (Model WA-01).
4. Click 'Programs' toolbar button - 'Programs' window will be opened.
5. Click 'New' button to create a new program associated with the selected machine model and fill-in the next obligatory fields:<br>
  - **Program:** P1<br>
    (for the rest of programs of the same machine model: P2, P3, P4 etc.)
  - **Type:** Normal
  - **Internal no.:** 1<br>
    (for the rest of programs of the same machine model: 2, 3, 4 etc.)
  - **Cycle duration (m):** 30<br>
  Then click 'Save' button, specify the program name (Program 1 (WA-01)) and save the change.
6. Repeat step 5 as many times as you need to have a different programs for the specified machine model.
7. Repeat steps 3-6 to add new programs to the rest of washers.
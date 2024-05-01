# Console Application for Processing Admission and Scholarship Letters

 <strong>Overview</strong>
 
 This console application processes daily admission and scholarship letters, archives them, and combines letters for students who have received both types. It then generates a report of combined letters.

 <strong>Setup</strong>

<strong>Directory Structure</strong>

Ensure that the directory structure is set up as follows:

- CombinedLetters
   - Input
     - Admission
        - 'yyyymmdd' format folder
     - Scholarship
        - 'yyyymmdd' format folder
   - Archive
   - Output


<strong>File Generation</strong>

Use the RandomFileGen.py script to generate sample admission and scholarship letters. Modify the path on line 5 of the script to point to the appropriate directories under the Input folder. The script will create 10 files for each, labeled with a random university ID and containing a placeholder text.

<strong>Moving Files</strong>

After generating the files, manually move them from the Input directory to a subdirectory named with the current date in yyyyMMdd format under both the Admission and Scholarship folders.

<strong>Configuration</strong>

In Program.cs, update the path for the root directory at line 33 to match your local setup. This ensures the application points to the correct root directory of your structured folders.

<strong>Running the Application</strong>

To run the application, compile and execute Program.cs. Ensure your development environment is set up to compile C# projects, such as having .NET SDK installed and configured.

<strong>Testing(test cases)</strong>

1. ___Current Date Processing:___ Test by running the code on the current date after setting up the folders correctly. This should process the letters for today, archive them, and generate the output and report.
2. ___No Folder for Today:___ Test by not creating a folder for today’s date. The application should output a message indicating missing folders.
3. ___Backlog Processing:___ Modify the last report's date to test the application's ability to process data for previous dates that were missed; more randomly generated files are required for this in each "yyyymmdd" input folder.
4. ___Repeated Execution:___ Run the application twice on the same day to ensure it doesn't reprocess already handled data.
5. ___Accuracy:___ Make sure to change a few IDs in the Admission or Scholarship folder so that it can be confirmed that the code excludes those IDs that are not in both the Admission and Scholarship folders.

Ensure that after each test:
- The **Archive** folder contains all files processed for the date.
- The **Output** folder includes combined letters and the report.txt detailing the operations.

<strong>Notes</strong>

- Make sure Python is installed for running **'RandomFileGen.py'**.
- Adjust paths in the Python script and **'Program.cs'** as needed based on your environment.

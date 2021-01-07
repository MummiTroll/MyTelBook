# MyTelBook
Small private Telephone book / NOSQL-database

The app creates and supervises multiple databases in the format:
Surname
Middle name
Name 
Telephone
E.Mail
Address
City
Country
Birthday
Skype
VK
Homepage

Each field entry for a DB item is a string list. 

Import DB: each database can be imported from txt, xls, xlsx, docx and pdf files, and updated from the same file formats or manually from the screen formular (Manual update). The “Extensions” ComboBox defines the file extension. File names for updates can be chosen through the dialogue field “Import/Export DB” ot typed.

Correct item: DB items can be corrected on the screen, followed by replacement in the selected DB. Surname of the item to correct is placed in the field “Import/Export DB”.

Delete Item: the item of the selected DB, indicated in the field “Import/Export DB” will be deleted.

DB: shows the content of the selected DB on the screen.

List items: to save output time gives a sorted list of surnames of the selected DB on the screen.

Export DB: each database can be exported to txt, xls, xlsx, docx and pdf files. The “Extensions” ComboBox defines the file extension. The directory for export is either typed in the field “Import/Export DB”, otherwise it will be placed in the directory: TelBook drive:\_TelBook\DB\Export\. DB export filename: “DBname_day.month.year_hour.min”.

Optimize DB: DBs can be optimized, the keys of the entries with the same surname get an index in brackets after the key.

Clear: erazes the  screen.

Test DB: creates a random DB, the number of items should be placed in the field “Import/Export DB” (administrator function).

Backup DB: saves the actial selected DB in the directory “\_TelBook\Backup\” (administrator function).

Restore DB: restores the selected DB from the directory “\_TelBook\Backup\” (administrator function).

Search: makes a search of the keyword from the field “Keyword” in the selected DB, alternatively use the “Enter” key. Search stops when the number indicated in the “hits” firld is reached. To enable search also with mistyped keywords, after the direct search, if the indicated nuber of hits is not reached, the “Extended search” algorithm is run. This algorithm implies alternated shortening the keyword from both termini till the length indicated in the “Minimal window” field.

When the CheckBox “All in one” is checked, only the DB entries will be considered to be hits, which contain all the keywords from the field “Keyword”, separated by comma.

Save: saves the result of the current search in \_TelBook\SearchResults\ in a file “Search_results_day.month.year_hour.min.txt”. 

Delete DB: Deletes the selected DB (administrator function).

Set password: sets the administrator password  (administrator function).

Admin CheckBox: makes the buttons of the administrator functions (red) visible. In the final version will be made invisible and available upon a use of a certain schorcut.

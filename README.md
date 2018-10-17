# WebControl
A quick tool I am putting together to automate some web scraping and data entry. The purpose of the tool is to enable embedded @remote on a Ricoh fleet via the WIM over the clients network.  It's unofficial and unsupported. 

This tool will take an 2 csv files (iplist.csv & codes.csv by default). The CSV files are to be 1 column only per file. It uses /n and /r characters to split the Ip's and codes. If they are more than one column it will not be able to parse the file properly.

It will run 10 threads simultaniously(currently hard coded) within the chorme browser until iteration through the Ip list completes. It iterates through the Ip list from the top down. If you have to stop it midway find the last Ip logged and delete all of the rows above it.

I coded NO logic to verify that enough codes are present in the codes.csv file, please make sure you provide sufficent codes or it will probably just stop working if it runs out. It works through the codes CSV file from the bottom up. If you have to stop it find the last code used and erase everything below it.

During every log event it will save the two tables automatically to the working directory(complete.csv and error.csv) be sure to save them before running the next batch as it will overwrite them.

Upon detecting an error during its attempt to activate @remote it will save a screenshot to the \ss\ directory in the .exe working directory.

# Advent of Code
This repository contains my solution for the Advent of Code Advent calendar (https://adventofcode.com/) It also contains the Inputs, that were given to me.

The following explains the structure of the Repository and how to run it.

## AdventOfCode
The Console Application is supposed to act as a launcher for every day.
It loads the desired year from a DLL and allows you to choose a day.
As of writing the desired DLL cannot be chosen and a command line argument needs to be provided for it.

## AdventOfCommon
This project represents a common library with tools that can be used for all years.
Additionally it contains the DayBase class, that all days of all years need to implement.

## AoC2019
My solution for Advent of Code 2019. It's not even finished and due to changes in the DayBase, cannot be executed at all.
it was also my first take on Advent of Code.

## AoC2020
My first completed Year. It is fully functional.

## Running Advent of Code
Provide the Application with a Command line argument pointing to the directory which contains the desired Advent of Code DLL (Must be named AoC[year].dll) file with its inputs in an "Inputs" subdirectory.
The Directory can only contain a single DLL right now.

If the DLL was found, the console will greet you with the detected year number and promt you to enter a date. if the Day was found it will require you to select a part or a part in test mode.

When in test mode, you can provide the input via the console. In that case, replace the line breaks with a printed \n.
The console Buffer may be limited and not be able to hold the full example input in every case.

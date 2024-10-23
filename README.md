```
Author:     Khoa Minh Ngo and Duke Nguyen
Start Date: 14-Apr-2024
GitHub ID:  Mkhoa161 & duke7012
Commit Date: 25-Apr-2024
Solution:    Agario
```

# Overview of the Agario Solution Architecture

This document outlines the structure of the Agario solution, which follows the MVC architecture. 
The solution is divided into several projects, each responsible for different aspects of the application.

## AgarioModels

This Core Library project contains all the model code necessary for representing the game "world". 
This includes the management and storage of all game objects. The models defined in this project 
are reusable and can also be utilized in server-side code for game logic processing.

## ClientGUI

This is a Core MAUI project that includes all the graphical user interface code. 
- **Dependencies**:
    - **AgarioModels**: References the models project to access the game world and objects.
    - **Communications**: Utilizes the networking code to handle data transmission.

## Communications

This Core Library project houses the `Networking.cs` code, 
which is crucial for handling all network-related operations.

## Logger

This project includes file logger code, which is used across the solution to 
facilitate debugging and tracking of runtime operations. Logging is implemented 
within each project to capture essential information and errors.

## libs

- **Purpose**: If the custom `Networking.cs` code is non-functional or absent, a pre-compiled communications DLL will be used.
- **Instructions**:
    - Create a `libs` folder within the solution directory.
    - Place the communications DLL within this folder.
    - Link this DLL to the necessary projects to replace the custom networking code.


# Design Decisions:

In the design part of the Login GUI, we decided to have a somewhat similar layout as the previous assignment.
However, this time will have only the two necessary panels (as users only use ClientGUI):
        
1. The one below the header is used for displaying the connecting status with a dynamic button that makes a 
Client can connect OR disconnect to a custom address OR reconnect from an unsuccessful connection. 

2. The top right corner is used as a place for user to input their custom name and the server address (Server),
    or the custom address for connection (on Client's UI). The name customization feature is only available once
    the connection is successfully established, and the address customization feature is only available when the
    connection is NOT yet established.

In the Playing GUI, we decided to draw somewhat identical with what our professor's ClientGUI example.
In constrast, instead of zooming out of the world like in the example. We decided to display a GameOver
screen with some game statistics - including the current score, alive time as well as its own records in
a connection - together with the restart game button for the player have an ability to restart the game.


# Implementation Notes (optional)

    For this game, since the color of each player is assigned randomly and the text color is black with centered position, 
    the name may not be visible for all players in some cases.

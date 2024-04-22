```
Author:     Khoa Minh Ngo and Duke Nguyen
Partner:    None
Start Date: 14-Apr-2024
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  Mkhoa161 & duke7012
Repo:       https://github.com/uofu-cs3500-spring24/assignment-eight-agario-2-arctan
Commit Date: 25-Apr-2024
Solution:    Agario
Copyright:  CS 3500, Khoa Minh Ngo and Duke Nguyen - This work may not be copied for use in Academic Coursework.
```

# Overview of the Agario Solution Architecture

This document outlines the structure of the Agario solution, which follows the MVC architecture. 
The solution is divided into several projects, each responsible for different aspects of the application.

## Projects

### AgarioModels

This Core Library project contains all the model code necessary for representing the game "world". 
This includes the management and storage of all game objects. The models defined in this project 
are reusable and can also be utilized in server-side code for game logic processing.

### ClientGUI

This is a Core MAUI project that includes all the graphical user interface code. 
- **Dependencies**:
  - **AgarioModels**: References the models project to access the game world and objects.
  - **Communications**: Utilizes the networking code to handle data transmission.

### Communications

This Core Library project houses the `Networking.cs` code, 
which is crucial for handling all network-related operations.

### Logger

This project includes file logger code, which is used across the solution to 
facilitate debugging and tracking of runtime operations. Logging is implemented 
within each project to capture essential information and errors.

### libs

- **Purpose**: If the custom `Networking.cs` code is non-functional or absent, a pre-compiled communications DLL will be used.
- **Instructions**:
  - Create a `libs` folder within the solution directory.
  - Place the communications DLL within this folder.
  - Link this DLL to the necessary projects to replace the custom networking code.


# Partnership 

All the code in our project was written using pair programming. Khoa took the lead as the primary coder for the Zooming feature and Betworking
sections, while Duke took the lead in the main coding responsibilities for the ClientGUI and Drawing the map. We adopted a 
collaborative pair programming approach, with Khoa and Duke frequently switching roles as coder and driver to maximize efficiency 
and ensure equal contribution to all aspects of the project. This balanced division of tasks allowed us to effectively manage the workload 
and deliver a well-rounded solution.

# Branching

Since we practiced pair programming throughout the project, we opted not to utilize branching as we collaborated on all aspects 
of the code together. This approach allowed us to maintain close communication and ensure that both partners were actively involved 
in every step of the development process.


# Testing

To thoroughly test our entire program code, we conducted numerous trial and error runs for each small part of the assignment's requirements. 
For each preliminary task (TowardAgarioStepOne, TowardAgarioStepTwo, and TowardAgarioStepThree), we designated every "Run the program..." 
instruction as a checkpoint. This allowed us to track our progress according to the guidelines specified in each pre-assignment. We moved 
on to the next steps only when the outcomes aligned with our expectations. This approach was consistently used for each minor 
requirement/functionality specified in the main program (Section 10.3). It was crucial to ensure that we could easily revert to previous 
versions of our code whenever an added feature failed to function as intended.


# Examples of Good Software Practice (GSP)

- Documentation: Writing clear and concise comments within the code to explain complex sections or clarify the purpose of specific lines. Maintaining external documentation (my own note) to provide an overview of the project.
- Version Control: Using version control systems (e.g., Git) to track changes, and maintain a history of the codebase.
- Designing code with reusability in mind, creating generic functions or classes that can be easily adapted for different use cases.
- Time management: We divided into equal code sessions to make our work more efficient and could track easily of what we have done.

# Time Expenditures:

    Assignment Eight:   Predicted Hours:          30        Actual Hours:   24
        
        Khoa: 24 hours (24 hours collaborative)
        Duke: 24 hours (24 hours collaborative)

        Breakdown:
            Assignment Preparing: 4 hours (estimated)
            Making progress: 12 hours (estimated)
            Testing & Debugging: 8 hours (estimated)

        
        Reflection on Estimates:
            Our initial estimate of 30 hours for this assignment provided a baseline expectation for the effort required. 
            However, the actual time spent of 24 hours indicates that our ability to accurately estimate project timelines 
            is still developing. The additional time invested in learning tools and techniques, debugging, and testing 
            highlights areas where we may need to improve our efficiency or deepen our knowledge. This experience underscores 
            the importance of ongoing self-assessment and reflection to refine our estimation skills and better gauge our abilities.

        Reflection on Time Allocation:
            Analyzing the breakdown of time spent on different aspects of the project reveals valuable insights into our abilities 
            and areas for improvement. While the majority of our time was dedicated to making progress on the assignment, the significant 
            portion allocated to learning new tools and techniques suggests a potential need to enhance our proficiency in certain areas. 
            Additionally, the time spent debugging underscores the importance of thorough testing and code review processes to minimize 
            errors and streamline development. Moving forward, we will leverage these insights to enhance our capabilities and optimize 
            our approach to future projects.


# Design Decisions (optional)
        In the design part, we decided to have a somewhat similar layout as the given example (SimpleClientAndServer).
        Both Server and Client UI are mostly identical, which are divided into 4 main parts (in a 2x2 grid).
        
        1. The top left corner is used for displaying the connecting status with a dynamic button that makes you
            can EITHER start OR stop the server (Server), and a Client can EITHER connect OR disconnect to a custom
            address OR reconnect from an unsuccessful connection. 

        2. The top right corner is used as a place for user to input their custom name and the server address (Server),
            or the custom address for connection (on Client's UI). The name customization feature is only available once
            the connection is successfully established, and the address customization feature is only available when the
            connection is NOT yet established.

        3. The bottom left corner is used for sending and receiving messages. The chat history will be cleared once
            a new connection is established, and keep updating until the termination of the local application.

        4. The bottom right corner is used for displaying a list of current participants (clients) in the current server.
            The list is continuously updated over the connecting time until there is a disconnection. This will include
            the updates of incoming (adding) and disconnecting (removing) clients, as well as name changes of each client.
            Please refer to implementation notes for how the display of the participants list works.
        

# Implementation Notes (optional)
        This is an implementation notes for the command messages part. As per the requirements of this assignment, it is
        unclear that whether it is suitable for mainly automatic command messages (auto-send a command message corresponding
        to the appropriate action), OR manually syntax-based messages submitted by the users.

        Therefore, in the efficiency term, we chose to implement the automatic sending command messages. This is because
        we believe that easier actions (such as filling the updated name, constantly updated participant list, ...) should
        be easier than remembering the command messages to get them inputted into the chat.

        The functionality will do the same job as described in the assignment. For example, the "Command Name [name]" one
        will be triggered when the user pressed "Update" button for changing names.
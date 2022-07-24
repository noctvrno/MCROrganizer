# Multiple Challenge Run Organizer - MCRO

Currently being developed by one person - that's me :) - using C# WPF. Below you'll have a short description of the tool, its use and some planned features.
- MCRO is being primarily designed for challenge runners that stream their misfortune to a live audience.
- MCRO was designed to be captured in OBS and placed above existing splits.
- MCRO is meant to ease the organization of multiple games that need to be completed in order to complete a run.
- Streamers usually do this by having some photoshop templates that they edit everytime they complete a game (no-hit runners especially, as speedrunners do not bother with this because it is tedious and they just tend to plop every split into a huge list and it looks terrible).

# Demonstration
There are two main modes of the application: Classic mode and Moden mode.
**Classic Mode** features only rectangular shapes for displaying runs that have borders. The colors of the borders, backgrounds and fonts can be fully customizable. The size parameters can also be changed, but bear in mind that this is a global effect and it will apply to all runs.

With **Modern Mode** you can attach any image in place of the default ones that will represent the pending, in progress and finished runs. This mode was designed with the idea in mind that all runs will still have the same Width and Height, ensuring the best possible **alignment** and **quality**. MCRO will allow the addition of inconsistent image resolutions, but be warned that the results may not be good looking and the workflow might be buggy.

Each run has **3** possible states it can be in:
- Pending: the run has not been started yet; it is marked with the color **red** by default.
- In Progress: the currently active run, the game that you are playing at this moment; it is marked with the color **yellow** by default.
- Finished: the run has already been completed; it is marked with the color **green** by default.

### Run manipulation: adding, deleting, renaming and moving runs around
![RunManipulation](https://user-images.githubusercontent.com/63927668/178163180-14c89f91-1f21-405c-9a83-d4ffac0e946c.gif)

### Editing the size of the runs
![EditRunSize](https://user-images.githubusercontent.com/63927668/178163208-dce7bb4b-c178-4fc5-9263-219cc52faa0a.gif)

Edit the width, height and spacing for each run. Each of these parameters are computed to work within a range (based on the other parameters and the width of the window).

### Setting a logo for a run
![SetRunLogo](https://user-images.githubusercontent.com/63927668/178163235-59796ae1-0d27-4c0b-9f7d-c744dc87b1b2.gif)

Logos must be properly dimensioned beforehand, you will not be able to resize them in the application currently.

### Setting a background for the application:
Right click to bring up the global run settings and select "Change background image".

### Modern Design Workflow
![ModernMode](https://user-images.githubusercontent.com/63927668/179598147-ccb8940c-640a-4df2-a196-8de5c3d21d12.gif)

### Others
- You can save the current ChallengeRun as an .MCRO file and Load it the next time you stream. This saves everything you have changed/added in the current template for easy reusability.
- For OBS users: you can make the application's background transparent by adding a Color Key as an effect on the captured window. Play around with the settings in order to achieve the best result.

### Future implementations:
- Classic mode: Transparent backgrounds and borders for the runs - Not difficult to implement.
- Classic mode: Add new fonts and font sizes - Not difficult to implement.

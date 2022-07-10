# Multiple Challenge Run Organizer - MCRO

Currently being developed by one person - that's me :) - using C# WPF. Below you'll have a short description of the tool, its use and some planned features.
- MCRO is being primarily designed for challenge runners that stream their misfortune to a live audience.
- MCRO was designed to be captured in OBS and placed above existing splits.
- MCRO is meant to ease the organization of multiple games that need to be completed in order to complete a run.
- Streamers usually do this by having some photoshop templates that they edit everytime they complete a game (no-hit runners especially, as speedrunners do not bother with this because it is tedious and they just tend to plop every split into a huge list and it looks terrible).
- MCRO represents each game by using rectangular shapes - I do understand that maybe some people might like triangles or other funny shapes... well, tough luck.
- The tool will also contain features such as:
    - Simple tool => simple interface - no buttons or unnecessary dialogs, edit everything in one window.
    - Basic functionalities such as adding, deleting, editing a run (its name so far) - implemented.
    - Setting the added runs in a preferred position simply by dragging and dropping the run - implemented.
    - Dynamically updated runs (this includes the possibility to modify the spacing between the runs, their width and height) - implemented.
    - Loading and saving templates with runs that are meant to be completed (so that the user does not create the runs everytime the program is booted) - implemented.
    - Shortcuts for ease of use (go to next run, go to previous run) - future implementation.
    - Randomize the order of the runs - future implementation.
    - Color customization for the state of the runs, fonts, font size, font weight and other cosmetic stuff - future implementation.
    - TBA.

## Demonstration

### Run manipulation: adding, deleting, renaming and moving runs around
![RunManipulation](https://user-images.githubusercontent.com/63927668/178163180-14c89f91-1f21-405c-9a83-d4ffac0e946c.gif)

### Editing the size of the runs
![EditRunSize](https://user-images.githubusercontent.com/63927668/178163208-dce7bb4b-c178-4fc5-9263-219cc52faa0a.gif)
Edit the width, height and spacing for each run. Each of these parameters are computed to work within a range (based on the other parameters and the width of the window).

### Setting a logo for a run
![SetRunLogo](https://user-images.githubusercontent.com/63927668/178163235-59796ae1-0d27-4c0b-9f7d-c744dc87b1b2.gif)
Logos must be properly dimensioned beforehand, you will not be able to resize them in the application currently.

### Others
- You can also customize the color of the borders, backgrounds and also the font color.
- You can save the current ChallengeRun as an .MCRO file and Load it the next time you stream. This saves everything you have changed/added in your current template.

### Future implementations:
- Transparent backgrounds and borders for the runs - Not difficult to implement.
- Add new fonts and font sizes - Not difficult to implement.
- Design your own run display and reference them inside the application (kind of how Run Logos work right now - you add an image as the background of each run) - Not difficult to implement.

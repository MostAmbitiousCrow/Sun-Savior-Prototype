# **Week 2:**

### **Tuesday 15th October**

This week, I setup my initial GitHub repository. I added a newly built Unity Project under the version 2022.3.46f1 and used the Universal 3D Render Pipeline Template since it offers the best optimised graphics across different platforms.

### **Thursday 17th October**  

I created a text file to store my Learning Journal and some empty text files to use when documenting my written tutorial.

I then investigated on how to create formatting for my notes to make it easier to read thanks to this article:
[https://docs.github.com/en/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax](https://docs.github.com/en/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax)

I then broke down the fundamentals of my prototype of what will be included in the written tutorial.

<img width="630" alt="Screenshot 2024-10-17 at 13 05 17" src="https://github.com/user-attachments/assets/37298290-10b7-4f41-b01f-db92c6717db4" />

I then discovered a brilliant app called **Obsidian**. **Obsidian** is a free programmers note app which uses the same written formatting techniques used in GitHub! So I can easily take notes here without having to constantly push changes and clutter history.

![Pasted image 20250108163224](https://github.com/user-attachments/assets/9cf0fd4e-0a09-424f-add8-593f0eaa172c) 

### Saturday 19th October

I began working on my Camera Controller script. The aim for this script was to get the camera to pivot around the tower in a direction based on whether the player was aiming their cursor on the left or right sides of the screen.

Obviously I had to create zones for this. So I created a way to determine a percentage of the users screen which, if their cursor was within that area, the camera would move in that side of direction (aim cursor left to rotate the camera left, aim cursor right to rotate the camera right).

I utilised Tooltips and Regions since I wanted to stay organised and deliver as much context and mapping for my scripts, in case I needed to return back to them or forgot what they did.

I then finalised it by adding a nice smoothing effect to it, along with a speed control for the horizontal smoothing.

However, I ran into lots of issues when implementing the **Birds Eye View** mechanic (This mechanic allowed the player to position their cursor at the top of the screen, causing the camera to pivot upwards to give a top-down view of the tower).

Once of these issues I'm dealing with is where exiting birds eye view causes the camera to bounce before reaching. This could be because the coroutine is triggering twice since it always bounces to the target at the same pace it transitioned towards it. 

**Update:** it seems to return half-way through, then slowly Lerp towards the end target again. So it's something to do with either the timer or the way the rotation is being calculated. Though, it's something that I can't seem to figure out so I might just rework it to become a button input instead.

**Update 2:** Fixed it!
My solution was to rework the horizontal camera movement/rotation to update a float value, representing the Y axis of the objects rotation, and apply that value with the vertical camera movements, which represented the X axis, to a new rotation.

Though, I'm unsure what was causing that bouncing effect though. One theory could be that it was because of the fact that, to make the smoothing effect work, I need to keep updating the cameras rotation, even when not in the cursor trigger zone. This caused the script to update both the birds eye transition movement and smoothing at the same time, possibly giving that camera bouncing effect.


# Week 3:
### Monday 21st October

Today, I began my Unit_Place_Controller script. The aim of this script was to allow the player to create turrets on the ground at where ever their cursor was pointing at. This would first create a 'ghost' object of the turret which would appear if the player holds the right mouse button, then create the turret when they click the left mouse button.

However, I did encounter one issues which was that the ghost object would flicker whenever I moved it. I think this had something to do with the positioning of the raycast to the geometry

The following aspects have been considered such as: detecting if the player is aiming at 'Ground' labelled geometry, setting the objects Y position to properly match the placement so it sits directly on the surface of the geometry.

### Tuesday 22nd October

Today, I got some extremely useful notes from my Programming Lecturer (Hi Paul) about how I can improve the content and readability of my tutorial. I'll leave these here to share and compare if they made it into the final tutorial:
Tutorial Feedback:

* Include a “What you should know” section in the introduction.
This could be explaining what [Tooltips] do, and what [SerializeField] does as long as it’s relevant to the script. 
Otherwise, it could be helpful information to teach someone new to code, what these things do.

* You should also include a section talking about what the tutorial is about and give links to each tutorial page.

* Maybe give each page a context area at the start of the page?

* You could include tips using >[!tips] if something related to the script can be used to help someone with coding in the future!

* Add 'cs' to every code block and it will transform it into C#.
```cs
private float value = 0; // Example
```

* Make sure you’re not adding too many tutorial pages! It’s meant to be simple, so try to limit the amount of scripts you have in total.

* Add areas for mistakes, meaning that any section in the tutorial where some people might struggle or create a mistake, clarify what that issue would be and give a short explanation to where they might’ve made a mistake.

* Add page break zones to allow the user to scroll the page and avoid spoilers.

* You should only have four tutorials. So, in your case, One tutorial for player camera movement, One tutorial for placing and writing the AI for the player turret (and possibly selecting an extra unit), One for Enemy Ai and Wave Controller, And the finally the Wave Controller

I did encounter a small problem today, I had forgot to push my latest changes on GitHub yesterday and ended up having to work on an old version of the main branch.

I ended up creating scripts for the enemy AI, movement for the player turrets projectile and a health script that I can attach to every object I want to have health and receive damage.

However, when I got home, I encountered a clash caused by a modification of the main scene. In an attempt to merge anyway, I created a temporary branch where I applied the changes. It worked, but the contents of the scene was removed. So I instead created a Unity Package with all the new assets, and imported them to the main branch thus, in a way, solving my problem.

### Saturday 26th October

Today I continued on some of my scripts. Mainly the Unit_Place_Controller script where I backtracked a bit and did a few modifications to it. Mainly adding functionality for the box collider to detect if the player has enough room to place a turret, and improved how the raycast detects flat ground using the **Approximately()** function and how the positioning and rotation is set using **SetPositionAndRotation()** functions, which I had never used before (it essentially sets both the given transform components position and rotation, given its values).

I also changed how the UI looks for selecting a player turret/unit, and am now anticipating adding extra functionality so it can be hidden by pressing a button or key, and units can be selected with numbers 1 and 2.

<img width="819" alt="Screenshot 2024-10-26 at 17 45 57" src="https://github.com/user-attachments/assets/936d818b-4463-4b3a-a421-80cc3e631791" />

### Sunday 27th October

Today I went back to my first tutorial and updated it since I had made some changes to the original script a while ago. I also took the time to add more tips and information to the tutorial since it's becoming somewhat more advanced than it was originally.

I want this tutorial to be both a learning experience to using new coding techniques and learning how to create these scripts.

Though, I'm concerned I'm cramming in too much into these tutorials since, as of right now, I've added singletons to a **GameManager** script which contains money I want the player to earn from beating enemies and spend on purchasing turrets, a unit selection which, once completed, will allow the player to choose their tower to place. Though, this might be something I'd finish and implement towards the final prototype.

I know this won't be too difficult to implement since I've got a system planned for it. But I have a feeling the **Tower Units Tutorial**, tutorial will become too long...

Despite that, if I'm unsure if the tutorial will work or not, I'll take the time to try and follow and re-read it to see if I've made any mistakes.

I also added a lock feature to the Birds Eye View which allows the user to essentially lock the birds eye view perspective and continue moving their cursor around without exiting it. This is to solve the issue where someone might incorrectly place a turret on the ground and place it in an inaccurate area. So, allowing the birds eye view to stay should give the user more accuracy when placing units.

# Week 4

### Tuesday 29th October

Today, I continued the player turret script that I setup last week. I felt that the functionality to it was pretty basic, so, I decided to add different states to the turret. These states include: First (The turret targets the first enemy to enter its radius), Last (The turret targets the last enemy that entered its radius) and Closest (Targets the closest enemy within its radius).

One of these states was a "Strongest" state, where the turret would only target the closest enemy with the highest HP. However, this state has currently been put on hold because of time and technicality.

Originally, I had the script change state based on if the State variable would change. This was being detected every frame. However, I decided to, and for the sake of performance, change this to be a single event. Therefore, I can simply update the turret when the object is created, defaulting it to the First State, and then, I could possibly allow the player to click on the turret (Or possible UI) to change its State.

# Week 5

### Tuesday 5th November

Today I decided to push the project a bit more and added a way for the player to select a Unit and then place it. This would outcome to the player being able to click a button, representing a Tower/Player Unit, and it would select that Unit, ready for placement. It also changes the ghost to represent that Unit too.

<img width="1859" alt="Week 5 Turret + Wall Unit Showcase" src="https://github.com/user-attachments/assets/fa2ce71f-869d-41b6-9a53-fca4be3360b8" />

I was able to utilise the power of Unity Events again. However, I was unable to call the **Unit_Place_Controller**'s enum variable via the button component. So, unable to find a solution, I instead used integers to determine the selected unit. 
### Sunday 10th November

Updated tutorial two. 
I also decided to move the **Player Unit Controller** section of the **Placing Player Tower Units** tutorial to the **Enemy Units tutorial**, which will be renamed to **Player and Enemy Units**.

### Tuesday 12th November

Today I began the fourth tutorial, the Wave Manager. While Tutorial two and three aren't completed, I found myself getting stuck on the enemy AI script. 
This problem was caused by the way I was utilising the raycast for the enemy, as to detect player Units. Since the ray was positioned at the centre of the enemy and pointed forwards in a thin line, it couldn't detect anything on the side of the enemy, causing the enemy to get collide with objects on its side and begin drifting.

Stuck, I decided to start the script for the Wave Manager.

By the end of the day I had created a starting point for me to work with when spawning the enemies such as a struct variable, containing variables to determine enemy count, time and which spawners the enemies will spawn at.

### Thursday 14th November

Today I continued the Enemy AI script and found a solution to the raycast issue I had on Tuesday. My problem before was that the ray didn't cover as much area to properly detect Player Units, causing it to collide with the object and spin off it's axis. However, using **Physics.OverlapBox**, I was able to fix this problem since I could define a shape to detect things, and as a bonus: without using a physics collider.

I wanted to share the Wave_Manager script with the spawned enemy object by adding a Unity Event listener to the enemy HP script. But it didn't quite work out. For some reason, when adding a listener to the hp Script, it simply wouldn't appear in the inspector, nor trigger...

So, as an alternative, I just stuck to assigning the Wave Manager Script Component to the enemy so it can trigger the Wave Manager to remove the enemy from the list.

My next step is to finish the player unit script and move on to finalising the Wave Manager and write the rest of the tutorials.



### Tuesday 26th November:

Today I experimented with Unity’s **OnValidate** event. This event is an Editor-only function which Unity will call whenever a script is loaded or a value is changed in the inspector.

I’m using this to restrict values to stay within a range and update any of values if another value was changed. This is to reduce the amount of errors and checks I’ll have to add to the script, one of these being, as an example, if the player has added more spawns than the number of sides the ground shape, the value would be changed to the value of the number of assigned sides of the ground shape.

### Tuesday 3rd December:

Completed the ‘active spawner’ restriction.

I added functionality to assigning the 'active spawner' variable, which essentially represents the spawner in the list of created spawners in the scene, to stop same spawner from being used in the spawn information struct.

However, I ran into an issue where I couldn't access and change and add the values from the struct variable containing the enemy spawn variables. However, I was informed that this was because I was using a struct variable, not a class. Using a public class allowed me to access and change and add the values from the list and the inspector.

Added a min/max spawn range variable and functionality to vary the enemy spawns which will spread out spawned enemies via the spawners local x axis as to prevent enemies from lining up when being spawned.

### Thursday 2nd January

Today, I returned to the project to make some final changes to the scripts and tutorials.

On Tutorial 2, I changed how the ghost object functions in the Unit_Place_Controller script. Previously, I'd have a Ghost Object for each Unit, but I've changed it to just one GameObject, the Ghost Unit, which will change Mesh and Material to represent the selected Unit.

I also created two models for the player units: the Basic Turret Unit and Wall Unit in Autodesk Maya so I can include them in the package and not have to build the units out of already existing meshes provided by Unity.

<img width="2032" alt="Screenshot 2025-01-03 at 13 29 56" src="https://github.com/user-attachments/assets/2a3ffcf7-a97b-486a-b32f-dd8f36db3c2c" />

<img width="2032" alt="Screenshot 2025-01-03 at 13 31 50" src="https://github.com/user-attachments/assets/6b10123f-eae8-4080-b166-5b90f09e765c" />


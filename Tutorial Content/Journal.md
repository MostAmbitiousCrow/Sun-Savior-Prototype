# Sun-Savior-Prototype Journal
 
# **Week 1:**

### **Tuesday 8th October**

I spent this week planning my Games Programming project. To do this, I created a Miro Board and constructed a Mind Map of the contents of the prototype. ==(Miro Board)==

I wanted to aim for something that aimed for something visually technical while also delivering unique gameplay. ==(Aim For the Project)==

So, I had the idea of a 3D tower defence game where you’re only able to build wherever there is light. This would utilise Unity’s lighting system and could also give the project a nice atmosphere if done correctly. ==(Prototype Concept)==

However, one concern I had was to do with the assets that could be required to make this prototype work visually.

That and I didn’t have too much experience working with 3D lighting in Unity before, so much research would be required. ==(Concern With the Prototype)==

I’d also have to limit the design of the enemy units and player units since getting the fundamentals down could be costly of my time. ==(Second Concern: Time)==

# **Week 2:**

### **Tuesday 15th October**

This week, I setup my initial GitHub repository. I added a newly built Unity Project under the version 2022.3.46f1 and used the Universal 3D Render Pipeline Template since it offers the best optimised graphics across different platforms. ==(Choosing my Template)==

### **Thursday 17th October**  

I created a text file to store my Learning Journal and some empty text files to use when documenting my written tutorial.

I then investigated on how to create formatting for my notes to make it easier to read thanks to this article:
[https://docs.github.com/en/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax](https://docs.github.com/en/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax)

I then broke down the fundamentals of my prototype of what will be included in the written tutorial.
<img width="630" alt="Screenshot 2024-10-17 at 13 05 17" src="https://github.com/user-attachments/assets/97cfd92e-0895-4bed-934f-15a161f3209f">

I then discovered a brilliant app called **Obsidian**. **Obsidian** is a free programmers note app which uses the same written formatting techniques used in GitHub! So I can easily take notes here without having to constantly push changes and clutter history. (Though, I could just do that on a separate branch...)

### Saturday 19th October

I began working on my Camera Controller script. The aim for this script was to get the camera to pivot around the tower in a direction based on whether the player was aiming their cursor on the left or right sides of the screen.

Obviously I had to create zones for this. So I created a way to determine a percentage of the users screen which, if their cursor was within that area, the camera would move in that side of direction (aim cursor left to rotate the camera left, aim cursor right to rotate the camera right).

I utilised Tooltips and Regions since I wanted to stay organised and deliver as much context and mapping for my scripts, in case I needed to return back to them or forgot what they did.

I then finalised it by adding a nice smoothing effect to it, along with a speed control for the horizontal smoothing.

However, I ran into lots of issues when implementing the **Birds Eye View** mechanic where, if the player held their cursor at the top of the screen, the camera would pivot downwards to give a top-down view of the tower.
Once of these issues I'm dealing with is where exiting birds eye view causes the camera to bounce before reaching. This could be because the coroutine is triggering twice since it always bounces to the target at the same pace it transitioned towards it. 

**Update:** it seems to return half-way through, then slowly Lerp towards the end target again. So it's something to do with either the timer or the way the rotation is being calculated. Though, it's something that I can't seem to figure out so I might just rework it to become a button input instead.

**Update 2:** Cool, fixed it!
My solution was to rework the horizontal camera movement/rotation to update a float value, representing the Y axis of the objects rotation, and apply that value with the vertical camera movements, which represented the X axis, to a new rotation.

Though, I'm unsure what was causing that bouncing effect though. One theory could be that it was because of the fact that, to make the smoothing effect work, I need to keep updating the cameras rotation, even when not in the cursor trigger zone. This caused the script to update both the birds eye transition movement and smoothing at the same time, possibly giving that camera bouncing effect.


# Week 3:
### Monday 21st October

At the time of writing, the first tutorial, 1. Camera Movement Tutorial is complete.

Moving on to the second tutorial now!


Mention Flickering (Ghost Turret would flicker when moving it around the ground. Was fixed by assigning the Y value to the surface of the mesh?)

Today, I began my Unit_Place_Controller script. The aim of this script was to allow the player to create turrets on the ground at where ever their cursor was pointing at. This would first create a 'ghost' object of the turret which would appear if the player holds the right mouse button, then create the turret when they click the left mouse button.

However, I did encounter one issues which was that the ghost object would flicker whenever I moved it. I think this had something to do with the positioning of the raycast to the geometry

The following aspects have been considered such as: detecting if the player is aiming at 'Ground' labelled geometry, setting the objects Y position to properly match the placement so it doesn't accidentally become too hight or spawn under the geometry?

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

I did encounter a small problem today, I had forgot to push my latest changes on GitHub yesterday and ended up having to work on an old version of the main branch. I ended up creating scripts for the enemy AI, movement for the player turrets projectile and a health script that I can attach to every object I want to have health and receive damage.

However, when I got home, I encountered a clash caused by a modification of the main scene. In an attempt to merge anyway, I created a temporary branch where I applied the changes. It worked, but the contents of the scene was removed. So I instead created a Unity Package with all the new assets, and imported them to the main branch.

### Saturday 26th October

Today I continued on some of my scripts. Mainly the Unit_Place_Controller script where I backtracked a bit and did a few modifications to it. Mainly adding functionality for the box collider to detect if the player has enough room to place a turret, and improved how the raycast detects flat ground using the **Approximately()** function and how the positioning and rotation is set using **SetPositionAndRotation()** functions, which I had never used before.

I also changed how the UI looks for selecting a player turret/unit, and am now anticipating adding extra functionality so it can be hidden by pressing a button or key, and units can be selected with numbers 1 and 2.

<img width="819" alt="Screenshot 2024-10-26 at 17 45 57" src="https://github.com/user-attachments/assets/0a071b89-bbfa-4723-9496-1b881491e57f">

### Sunday 27th October

Today I went back to my first tutorial and updated it since I had made some changes to the original script a while ago. I also took the time to add more tips and information to the tutorial since it's becoming somewhat more advanced than it was originally.

I want this tutorial to be both a learning experience to using new coding techniques . Though, I'm concerned I'm cramming in too much into these tutorials since, as of right now, I've added singletons to a **GameManager** script which contains money I want the player to earn from beating enemies and spend on purchasing turrets, a unit selection which, once completed, will allow the player to choose their tower to place. Then, I want to add a way for the player 

I know this won't be too difficult to implement since I've got a system planned for it. But I have a feeling the **Tower Units Tutorial**, tutorial will become too long...

Despite that, if I'm unsure if the tutorial will work or not, I'll take the time to try and follow it to see if I've made any mistakes.

I also added a lock feature to the Birds Eye View which allows the user to , this is to solve the issue where someone might incorrectly place a turret on the ground and place it in an inaccurate area. So, allowing the birds eye view to stay should give the user more accuracy when placing units.

# Week 4

### Tuesday 29th October

Today, I continued the player turret script that I setup last week. I felt that the functionality to it was pretty basic, so, I decided to add different states to the turret. These states include: First (The turret targets the first enemy to enter its radius), Last (The turret targets the last enemy that entered its radius) and Closest (Targets the closest enemy within its radius).

One of these states was a "Strongest" state, where the turret would only target the closest enemy with the highest HP. However, this state has currently been put on hold because of time.

Originally, I had the script change state based on if the State variable would change. This was being detected every frame. However, I decided to, and for the sake of performance, I would change this to be a single event. Therefore, I can simply update the turret when the object is created, defaulting it to the First State, and then, I can allow the player to click on the turret (Or possible UI) to change its State.


# MiniNoire
---
### Intro
A small dynamic music system implementation using Unity and FMOD for educational purposes by <a href="https://twitter.com/jbuaudio" target="_blank">Julie Buchanan</a>  and <a href="https://twitter.com/Dani_Kogan" target="_blank">Dani Kogan</a>
<br>
Where a Technical Sound Designer and a Composer switched roles. 
Watch the demo<a href="https://www.youtube.com/watch?v=LGvL7OBCZ_4&ab_channel=DaniKogan" target="_blank"> here </a>
### The Project
The aesthetics we chose for the music system was Mini Metro meets Noire. Hence is the name Mini Noire.
The system is supporting a detective gameplay and consists of three elements:
- The Player = 3rd person controller from the unity store
- Crime Scene = The orange area in the level
- Clue = The blue cubes in the level

### The Music System
| Player Action | Music Reaction|
|:-------------:|:-------------:|
| Enter level    | Music start |
| Enter crime scene      | Drum and Bass layer fade in      |
| Leave crime scene | Drum and Bass layer fade out      |
| Proxomity to a clue    | Clue music layer adjusted in volume |
| Collide with a clue    | Stinger is triggered on beat and in key with the music |



### Versions
Unity 2018.2.15f1
<br>
FMOD 1.10.09

1.0.1
 - Odin inspector is now a requirement. Can serialize IStep directly into the sequencer.

1.0.2
 - Added multistep, step addon base class, sequencer addon base class and other utilites.
 - Fixed sequencer to correctly evaluate forwards and backwards
 
1.0.3
  - Intent to fix problems with step event invoking and subscribing execution order
  
1.0.4
 - Fixed sequencer and it's event hook component
 
1.0.5
 - Fixed multistep backward evaluation 
 
1.0.6
 - Fixed multistep backward evaluation for real this time
 - Removed some debug logs

1.0.7
 - Multistep fix for real for real this time
 - Added some common steps for buttons and toggles

1.0.8
 - Multistep and sequencer take into account already completed steps upon starting the check
 - Hopefuly fixed assembly problems

1.0.9
 - Step code improvements
 - StepUIToggle fixes
 - Added sequencer logger component
 
1.0.10
 - Added SFX related sequence and step addons
 - Made base components abstract so that they can't be put in gameobjects
 
1.1.0
 - Improved sequencer inspector
 - Added automatic preprocessing for steps and sequences upon scene save

1.1.1
 - Changes sfx step to be easier to inherit and modify
 
1.1.2
 - Sequencer setup button renames children even if there are no steps
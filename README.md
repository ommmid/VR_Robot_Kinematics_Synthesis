# VR_Robot_Kinematics_Synthesis
This is a VR windows application with which one can do the serial robot kinematics synthesis.
  
The input is taken from user real hands through leap motion in the virtual environmnet. 
![picture1](https://user-images.githubusercontent.com/26231820/44758318-f9864780-aaf0-11e8-9cdd-779b1054fdeb.png)
Then the app sends the input to ArtTreeKS which is a robot kinemtaic synthesis solver. The kinematics equations are solved by this solver and the result which is links lengths and relative ablges in the form of quaternion is sent back to the app where the user can see the robot and change the configuration to see if it is passing through thoses desired poses or not. Using this app, the user does not need to know any advanced kinematics or robotics information. This app basically changes a graduate-level problem to a simple VR game that almost any one can play.
  
Packages used in this game are:
unity 2017.2
LeapMotion_CoreAsset_Orion_4.1.5
InteractionEngine-0.3.0
UIInputModule-1.2.0

The devices needed for the game are:
Oculus Rift
Leap Motion

Here is the demo of the app: https://youtu.be/YI5-mMtStik 

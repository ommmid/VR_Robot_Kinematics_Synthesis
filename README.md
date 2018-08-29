# VR_Robot_Kinematics_Synthesis
This is a Windows VR application with which one can do the serial robot kinematics synthesis. The input is taken from user real hands through leap motion in the virtual environmnet. Each pose is captured and a blue cube is placed in so the desired pose is displayed.

![picture1](https://user-images.githubusercontent.com/26231820/44758318-f9864780-aaf0-11e8-9cdd-779b1054fdeb.png)

Then the app sends the input to ArtTreeKS which is a robot kinemtaic synthesis solver. The kinematics equations are solved by this solver and the result, which is links lengths and relative anlges, in the form of quaternion is sent back to the app where the user can visualize the robot and change the configuration of it too see if it is passing through thoses desired poses or not. Using this app, the user does not need to know any advanced kinematics or robotics information. This app basically changes a graduate-level problem to a simple VR game that almost any one can play.
  
ArtTreeKS can only run in Linux operating system. So to resolve this problem Windows Subsytem fo Linux (WSL) was used to run the ArtTreeKS in the background. Unity communicate with ArtTreeKS through tcp/ip to send the input and take the calculated robot. Please install ArtTreeKS from this page https://help.cose.isu.edu/how-to/arttreeks then copy these files  


  
to WSL_directory\home\<username>\ArtTreeKS-master\examples\    .   
Run SocketServerC.lua in a terminal in WSL to run the server and then in Unity play the game to see the environment and be able to do the kinematics synthesis   .

Packages used in this game are:
unity 2017.2
LeapMotion_CoreAsset_Orion_4.3.4
InteractionEngine-1.1.1
UIInputModule-1.2.1

The devices needed for the game are:
Oculus Rift
Leap Motion

Here is the demo of the app: https://youtu.be/YI5-mMtStik 

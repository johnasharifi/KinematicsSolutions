# KinematicsSolutions

Kinematic and inverse kinematic solutions

## Theory

Given 
- a hierarchical collection of nodes with a position + forward
- a target with a position, and 
- a final node who we refer to as an End Effector

compute the rotations necessary to move the End Effector's position toward the target's position. 

To solve an inverse kinematics problem
- for each node, for each frame:
  - compute rotation necessary to rotate vector from-node-to-end-effector toward target
  - convert rotation from world space reference axes to local space reference axes
  - apply part of the local space rotation to the node

## Live web demo

https://johnasharifi.github.io/KinematicsSolutions/

## Related work

https://www.youtube.com/watch?v=ICoIFynmPzw

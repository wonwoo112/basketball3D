using Unity.VisualScripting;
using UnityEngine;

public class BallDribble
{
    public const float initUpperBound = 1.0f;
    public const float initBallSpeed = 1.5f;
    public const float minDribbleHeight = -0.5f;
    public bool isDribbling;
    private float upperBound;
    private float ballSpeed;
    private bool goingUp;
    private const float lowerBound = -0.9f;
    private bool leftDribble;
    private bool rightDribble;
    public Direction holdingHand;
    private float xSpeed;
    private const float leftPos = -1.0f;
    private const float rightPos = 1.0f;
    private const float acrossDist = rightPos - leftPos;
    public enum Direction {
        left, right
    }
    private Direction dribblingDirection;
    bool canDribble(Vector2 oldPos) {
        return goingUp && minDribbleHeight < oldPos.y && initUpperBound > oldPos.y;
    }
    void keyHandler(bool leftPressed, bool rightPressed, Vector2 oldPos) {
        if (leftPressed && canDribble(oldPos)) {
            leftDribble = true;
            rightDribble = false;
            dribblingDirection = Direction.left;
        }
        else if (rightPressed && canDribble(oldPos)) {
            leftDribble = false;
            rightDribble = true;
            dribblingDirection = Direction.right;
        }
        else {
            leftDribble = false;
            rightDribble = false;
        }
    }
    float targetX(Direction d) {
        switch (d) {
            case Direction.right: return rightPos;
            case Direction.left: return leftPos;
            default: return 0.0f;
        }
    }
    float getNewX(Vector2 oldPos, float dt) {
        // function for getting the new x position of the ball
        float oldX = oldPos.x, newX;
        if (holdingHand == dribblingDirection) {
            newX = targetX(dribblingDirection);
        }
        else {
            newX = oldX + xSpeed * dt;
        }
        return newX;
    }
    float getNewY(Vector2 oldPos, float dt) {
        // function for getting the new y position of the ball 
        
        float oldY = oldPos.y, newY;
        if (goingUp) {
            newY = oldY + ballSpeed * dt;
            
            if (newY > initUpperBound) {
                newY = initUpperBound;
                isDribbling = false;
            }
        }
        else {
            newY = oldY - ballSpeed * dt;
            if (newY <= lowerBound) {
                newY = lowerBound;
            }   
        }
        return newY;
        
    }
    void updateState(float newY) {
        // function for updating the state of the ball 
        if (goingUp) {    
            if (leftDribble) {
                upperBound = newY;
                if (holdingHand == Direction.right) {
                    xSpeed = -acrossDist / ((upperBound - lowerBound) / ballSpeed);
                }
                goingUp = false;
            }
            if (rightDribble) {
                upperBound = newY;
                if (holdingHand == Direction.left) {
                    xSpeed = acrossDist / ((upperBound - lowerBound) / ballSpeed);
                }
                goingUp = false;
            }
        }
        else if (newY <= lowerBound) {
            goingUp = true;
            xSpeed = 0.0f;
            holdingHand = dribblingDirection;
        }
    }
    public BallDribble()
    {
        goingUp = true;
        ballSpeed = initBallSpeed;
        upperBound = initUpperBound;
        isDribbling = false;
        xSpeed = 0.0f;
    }
    public Vector2 Update(bool leftPressed, bool rightPressed, Vector2 oldPos, float dt)
    {
        keyHandler(leftPressed, rightPressed, oldPos);
        float newY = getNewY(oldPos, dt);
        updateState(newY);
        float newX = getNewX(oldPos, dt);
        return new Vector2(newX, newY);
    }
}
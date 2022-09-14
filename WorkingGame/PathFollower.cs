using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace WorkingGame
{
    internal delegate void OnPathCompletion();

    internal enum PathFollower_MovementStates
    {
        Moving,
        ChangingDirection,
        Dead,
        Destination,
        ChasingPlayer,
    }

    /// <summary>
    /// Gives an object an ability to follow Path and respond to light by player
    /// </summary>
    internal class PathFollower
    {
        //state of PathFollower
        private PathFollower_MovementStates curState;

        public PathFollower_MovementStates State { get => curState; }

        //Position +offset
        private Vector2 position;

        public Vector2 Position { get => position + new Vector2(16, 16); }

        //speed
        private const float speed = 2f;

        private Vector2 velocity;

        //checkpoints and indexer
        private List<Vector2> checkpoints = new List<Vector2>();

        private int pathFollowerIndex;

        //is the pathfollower going to "original" beginning of list?
        private bool touchingTrueBeginning;

        //is the pathfollower going the right way? (prevents double event fires if already heading backwards)
        private bool headingRightWay = true;

        /// <summary>
        /// a target, once path is completed
        /// </summary>
        private Player target;

        /// <summary>
        /// event it fires once done completing whole path
        /// </summary>
        public event OnPathCompletion completed;

        public List<Vector2> Checkpoints
        {
            get => checkpoints;
        }

        /// <summary>
        /// gets reference to target after completing path
        /// </summary>
        /// <param name="player"></param>
        public void setTarget(Player target)
        {
            this.target = target;
        }

        private int WhileOnCollisionFrames = 0;
        private bool makeEventFireOnce = false;
        private bool chasingTarget = false;
        private bool TimerCompletion = false;
        private float Elapsed = 0f;

        /// <summary>
        /// Gives the behaviour for a gameobject to follow path and then chase player.
        /// </summary>
        /// <param name="position">Starting position of the path</param>
        /// <param name="pathComplete">Sound for path completion</param>
        public PathFollower(Vector2 position)
        {
            ///preps event listening
            this.position = position;
            curState = PathFollower_MovementStates.Moving;
            touchingTrueBeginning = false;
            LevelManager.OnCollisionEnter += PathFollower_OnCollisionEnter;

            LevelManager.WhileCollision += PathFollower_WhileCollision;
            LevelManager.OnCollisionExit += PathFollower_OnCollisionExit;
        }

        /// <summary>
        /// an Even that fire once pathFollower is no longer touching light
        /// </summary>
        /// <param name="enemy"></param>
        private void PathFollower_OnCollisionExit(Enemy enemy)
        {
            makeEventFireOnce = false;
            WhileOnCollisionFrames = 0;
        }

        /// <summary>
        /// while under collision count frames
        /// </summary>
        /// <param name="This"></param>
        private void PathFollower_WhileCollision(Enemy This)
        {
            if (this == This.PathFollower)
            {
                this.WhileOnCollisionFrames++;
            }
        }

        /// <summary>
        /// prevents repeat direction changes
        /// </summary>
        /// <param name="This"></param>
        public void PathFollower_OnCollisionEnter(Enemy This)
        {
            if (this == This.PathFollower)
            {
                if (this.makeEventFireOnce == false)
                {
                    if (headingRightWay == true && curState != PathFollower_MovementStates.ChasingPlayer && curState != PathFollower_MovementStates.Dead)
                    {
                        curState = PathFollower_MovementStates.ChangingDirection;
                    }
                    makeEventFireOnce = true;
                    if (this.curState == PathFollower_MovementStates.ChasingPlayer)
                    {
                        this.curState = PathFollower_MovementStates.Dead;
                    }
                }
            }
            else
            {
                //could do something for other listeners
            }
        }

        /// <summary>
        /// use to see if pathfollower is reaching next waypoint
        /// </summary>
        private float DistanceFromNextCheckPoint
        {
            get => Vector2.Distance(position, checkpoints[pathFollowerIndex]);
        }

        private float distanceOverall;

        /// <summary>
        /// returns percent enemy is toward final spot on list
        /// </summary>
        public float DistanceToFinalCheckPoint()
        {
            //Check for IsBeginning
            if (touchingTrueBeginning)
            {
                return (100 - ((Vector2.Distance(this.position, checkpoints[0]) / distanceOverall)) * 100);
            }
            else
            {
                return (100 - ((Vector2.Distance(this.position, checkpoints[checkpoints.Count]) / distanceOverall)) * 100);
            }
        }

        /// <summary>
        /// setups path
        /// </summary>
        /// <param name="checkpoints"></param>
        public void DefinePath(List<Vector2> checkpoints)
        {
            //pass in a path from file.io
            foreach (var point in checkpoints)
            {
                //add point to local list of checkpoints
                this.checkpoints.Add(point);
            }
            position = this.checkpoints[0];
            pathFollowerIndex = 0;
            distanceOverall = Vector2.Distance(checkpoints[0], checkpoints[checkpoints.Count - 1]);
        }

        /// <summary>
        /// Update loop
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime, Enemy This)
        {
            //movement based on stateMachine
            switch (curState)
            {
                case PathFollower_MovementStates.Moving:

                    ///if reaches end of collection
                    ///
                    if (pathFollowerIndex < checkpoints.Count)
                    {
                        //change index before reaching next destination, essentially unoticeable.
                        if (DistanceFromNextCheckPoint < 1f)
                        {
                            //if we are approaching next waypoint,
                            //set the new target to be next one in the list
                            pathFollowerIndex++;
                        }
                        else
                        {
                            //moving calculations
                            var direction = checkpoints[pathFollowerIndex] - position;
                            direction.Normalize();

                            velocity = Vector2.Multiply(direction, speed);
                            position += velocity;
                        }
                    }
                    else
                    {
                        if (touchingTrueBeginning)
                        {
                            //if heading towards beginning change direction so we go back towards the actual end
                            curState = PathFollower_MovementStates.ChangingDirection;
                        }
                        else
                        {
                            curState = PathFollower_MovementStates.Destination;
                        }
                    }

                    break;

                case PathFollower_MovementStates.ChangingDirection:

                    if (WhileOnCollisionFrames <= 0 || makeEventFireOnce == false)
                    {
                        try
                        {
                            touchingTrueBeginning = !touchingTrueBeginning;
                            headingRightWay = !headingRightWay;
                            Vector2 approaching = Vector2.Zero;
                            ///scuffed conditioning
                            if (pathFollowerIndex != 0)
                            {
                                approaching = checkpoints[pathFollowerIndex - 1];
                            }
                            else
                            {
                                approaching = checkpoints[pathFollowerIndex];
                            }

                            checkpoints.Reverse();
                            pathFollowerIndex = checkpoints.IndexOf(approaching);
                            //set back to moving
                            curState = PathFollower_MovementStates.Moving;
                        }
                        catch (Exception)
                        {
                            Console.WriteLine($"Index out of bounds approaching index: {pathFollowerIndex}");
                        }
                    }

                    break;

                case PathFollower_MovementStates.Destination:
                    completed?.Invoke();
                    curState = PathFollower_MovementStates.ChasingPlayer;
                    break;

                case PathFollower_MovementStates.ChasingPlayer:
                    chasingTarget = true;
                    FollowPlayer();
                    break;

                case PathFollower_MovementStates.Dead:

                    /* WAS ADVISED BY TEAM TO SCRAP AND CHANGE IT TO PAUSE, CHASE AND CHANGE COLOR.
                     pathFollowerIndex = 0;
                     position = checkpoints[pathFollowerIndex];
                     chasingTarget = false;*/

                    if (Timer(1.5f, gameTime) == true)
                    {
                        This.RandomColor();
                        WhileOnCollisionFrames = 0;
                        makeEventFireOnce = false;
                        this.curState = PathFollower_MovementStates.ChasingPlayer;
                    }
                    break;
            }
        }

        /// <summary>
        /// might replace later "lazy solution"
        /// </summary>
        public void FollowPlayer()
        {
            //takes slight ms
            if (target != null && chasingTarget)
            {
                //Math.Max(Math.Abs(destination.X - source.X), Math.Abs(destination.Y - source.Y));
                if (Math.Max(Math.Abs(target.X - position.X), Math.Abs(target.Y - position.Y)) > 1f)
                {
                    var direction = target.Position - position + new Vector2(16, 16);
                    direction.Normalize();

                    velocity = Vector2.Multiply(direction, speed);
                    position += velocity;
                }
            }
        }

        public bool Timer(float interval, GameTime gameTime)
        {
            Elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Elapsed >= interval)
            {
                Elapsed = 0;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
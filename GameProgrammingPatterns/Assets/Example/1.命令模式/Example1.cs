using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//没有优化的初始版本
//通用的Jump和Fire没有办法复用，耦合性高
namespace Example1_1
{
    public class Example1_1
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                Fire();
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                Jump();
            }
        }

        void Fire()
        {
        }

        void Jump()
        {
        }
    }
}

//将Jump和Fire单独剥离出来，调用的地方不用关心实现，既可复用又解耦了
namespace Example1_2
{
    public class Example1_2
    {
        public Command cmdFire;
        public Command cmdJump;

        void Start()
        {
            cmdFire = new FireCommand();
            cmdJump = new JumpCommand();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                cmdFire.Execute();
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                cmdJump.Execute();
            }
        }
    }

    public class Command
    {
        public virtual void Execute()
        {
        }
    }

    public class JumpCommand : Command
    {
        public override void Execute()
        {
            base.Execute();
        }
    }

    public class FireCommand : Command
    {
        public override void Execute()
        {
            base.Execute();
        }
    }
}

//让对象自身实现命令 而不是让命令来确定所控制的对象
namespace Example1_3
{
    public class Example1_3
    {
        public Command cmdFire;
        public Command cmdJump;
        public GameActor actor;

        void Init()
        {
            cmdFire = new FireCommand();
            cmdJump = new JumpCommand();
        }


        public Command InputHandler()
        {
            if (Input.GetKeyDown(KeyCode.A)) //可换成任意条件
            {
                return cmdFire;
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                return cmdJump;
            }

            return null;
        }

        public void TestFunc()
        {
            var cmd = InputHandler();
            if (cmd != null)
            {
                cmd.Execute(actor);
            }
        }
    }

    public class GameActor
    {
        public void Jump()
        {
        }

        public void Fire()
        {
        }
    }

    public class Command
    {
        public virtual void Execute(GameActor actor)
        {
        }
    }

    public class JumpCommand : Command
    {
        public override void Execute(GameActor actor)
        {
            actor.Jump();
        }
    }

    public class FireCommand : Command
    {
        public override void Execute(GameActor actor)
        {
            actor.Fire();
        }
    }
}

//撤销和重做
namespace Example1_4
{
    public class Example1_4
    {
        //如果想要支持多次撤销也很简单，维护一个命令列表就行了
        private Stack<Command> _cmdList = new Stack<Command>();


        /// <summary>
        /// 获取选中的物体，根据操作实现移动
        /// </summary>
        /// <returns></returns>
        Command HandleInput()
        {
            Unit unit = GetSelectedUnit();

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                int desY = unit.Y + 1;
                return new MoveUnitCommand(unit, unit.X, desY, unit.X, unit.Y);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                int desY = unit.Y - 1;
                return new MoveUnitCommand(unit, unit.X, desY, unit.X, unit.Y);
            }

            //其他方向....

            return null;
        }

        Unit GetSelectedUnit()
        {
            return null;
        }

        void TestFunc()
        {
            var cmd = HandleInput();
            if (cmd != null)
            {
                cmd.Execute();
            }
            
            //如果符合什么条件则撤销
            cmd.Undo();
        }
    }

    class Unit
    {
        private int _x;
        private int _y;

        public int X => _x;
        public int Y => _y;

        public void MoveTo(int x, int y)
        {
            _x = x;
            _y = y;
        }
    }

    public class Command
    {
        public virtual void Execute()
        {
        }

        public virtual void Undo()
        {
        }
    }

    class MoveUnitCommand : Command
    {
        private Unit _unit;
        private int _x;
        private int _y;
        private int _xBefore;
        private int _yBefore;

        public MoveUnitCommand(Unit unit, int x, int y, int xBefore, int yBefore)
        {
            _unit = unit;
            _x = x;
            _y = y;
            _xBefore = xBefore;
            _yBefore = yBefore;
        }

        public override void Execute()
        {
            _unit.MoveTo(_x, _y);
        }

        public override void Undo()
        {
            _unit.MoveTo(_xBefore, _yBefore);
        }
    }
}
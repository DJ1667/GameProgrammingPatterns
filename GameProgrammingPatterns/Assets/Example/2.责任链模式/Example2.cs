using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//场景
//现需要开发一个请假流程控制系统。
//请假一天以下的假只需要主管同意即可；请假2天需要项目经理同意；请假2天到4天的假还需要部门经理同意；请求4天到15天还需要总经理同意才行。

namespace Example2
{
    public class Application
    {
        public string Code;
        public string Description;
        public int Num;
        public string Type;
        public bool IsApproval;
    }

    //不带设计模式的实现
    namespace Example2_1
    {
        public class Example2_1
        {
            Application application = new Application()
            {
                Code = "Leave001",
                Description = "请假单20220715",
                Num = 16,
                Type = "请假单",
                IsApproval = false,
            };

            public void TestFunc()
            {
                if (application.Num <= 8)
                {
                    //Debug.Log("主管批准");
                }
                else if (application.Num <= 16)
                {
                    //Debug.Log("项目经理批准");
                }
                else if (application.Num <= 32)
                {
                    //Debug.Log("部门经理批准");
                }
                else
                {
                    //Debug.Log("总经理批准");
                }
            }
        }
    }

    //责任链模式
    namespace Example2_2
    {
        public abstract class AbstractManager
        {
            public string Name { get; set; }
            public abstract void Approval(Application application);
        }

        /// <summary>
        /// 主管
        /// </summary>
        public class Director : AbstractManager
        {
            public override void Approval(Application application)
            {
                if (application.Num < 8)
                {
                    //Console.WriteLine("主管审批通过！");
                    application.IsApproval = true;
                }
                else
                {
                    // Console.WriteLine("我没有权限,请找上级审批！");
                    ProjectManager manager = new ProjectManager();
                    manager.Approval(application);
                }
            }
        }

        /// <summary>
        /// 项目经理
        /// </summary>
        public class ProjectManager : AbstractManager
        {
            public override void Approval(Application application)
            {
                if (application.Num < 16)
                {
                    // Console.WriteLine("项目经理通过");
                    application.IsApproval = true;
                }
                else
                {
                    // Console.WriteLine("我没有权限,请找上级审批！");
                    DivisionManager manager = new DivisionManager();
                    manager.Approval(application);
                }
            }
        }

        /// <summary>
        /// 部门经理
        /// </summary>
        public class DivisionManager : AbstractManager
        {
            public string Name { get; set; }

            public override void Approval(Application application)
            {
                if (application.Num < 32)
                {
                    // Console.WriteLine("部门经理审批通过");
                    application.IsApproval = true;
                }
                else
                {
                    // Console.WriteLine("我没有权限,请找上级审批！");
                    President manager = new President();
                    manager.Approval(application);
                }
            }
        }

        public class President : AbstractManager
        {
            public string Name { get; set; }

            public override void Approval(Application application)
            {
                if (application.Num < 50)
                {
                    // Console.WriteLine("老板审批通过");
                    application.IsApproval = true;
                }
            }
        }
    }

    //责任链模式精髓
    // 进一步地，当我们要在项目经理和部门经理类直接加一个流转，或者要把项目经理的流转放在主管前面时
    // 我们就需要去修改各个管理者类的代码，这也就违背了开闭原则。所以，我们这里要使得流转的过程能够控制，

    namespace Example2_3
    {
        public abstract class AbstractManager
        {
            public string Name { get; set; }
            protected AbstractManager _abstractManager = null;

            public void SetNext(AbstractManager abstractManager)
            {
                this._abstractManager = abstractManager;
            }

            public abstract void Approval(Application application);

            protected void ApprovalNext(Application application)
            {
                // Console.WriteLine("我没有权限,请找上级审批！");
                if (this._abstractManager != null)
                {
                    this._abstractManager.Approval(application);
                }
            }
        }

        /// <summary>
        /// 主管
        /// </summary>
        public class Director : AbstractManager
        {
            public override void Approval(Application application)
            {
                if (application.Num < 8)
                {
                    //Console.WriteLine("主管审批通过！");
                    application.IsApproval = true;
                }
                else
                {
                    base.ApprovalNext(application);
                }
            }
        }

        /// <summary>
        /// 项目经理
        /// </summary>
        public class ProjectManager : AbstractManager
        {
            public override void Approval(Application application)
            {
                if (application.Num < 16)
                {
                    // Console.WriteLine("项目经理通过");
                    application.IsApproval = true;
                }
                else
                {
                    base.ApprovalNext(application);
                }
            }
        }

        /// <summary>
        /// 部门经理
        /// </summary>
        public class DivisionManager : AbstractManager
        {
            public override void Approval(Application application)
            {
                if (application.Num < 32)
                {
                    //Console.WriteLine("部门经理审批通过");
                    application.IsApproval = true;
                }
                else
                {
                    base.ApprovalNext(application);
                }
            }
        }
        
        public class President : AbstractManager
        {
            public override void Approval(Application application)
            {
                if (application.Num < 50)
                {
                    //Console.WriteLine("老板审批通过");
                    application.IsApproval = true;
                }
            }
        }
        
        public class ManagerBuilder
        {
            public static AbstractManager Build()
            {
                AbstractManager manager1 = new Director();
                manager1.Name = "Tom";

                AbstractManager manager2 = new ProjectManager();
                manager2.Name = "Bob";

                AbstractManager manager3 = new DivisionManager();
                manager3.Name = "Mark";

                AbstractManager manager4 = new President();
                manager4.Name = "Alice";

                manager1.SetNext(manager3);
                manager3.SetNext(manager4);
                return manager1;
            }
        }

        public class Example2_3
        {
            Application application = new Application();
            
            void TestFunc()
            {
                
                AbstractManager manager1 = new Director();
                manager1.Name = "Tom";

                AbstractManager manager2 = new ProjectManager();
                manager2.Name = "Bob";

                AbstractManager manager3 = new DivisionManager();
                manager3.Name = "Mark";

                AbstractManager manager4 = new President();
                manager4.Name = "Alice";

                manager1.SetNext(manager3);
                manager3.SetNext(manager4);
                manager1.Approval(application);
            }

            //我们如果不希望把业务流转的过程暴露在Program类中，我们可以引入建造者模型，在一个builder类中专门创建管理者类，并管理他们的流转方式。
            void TestFunc2()
            {
                AbstractManager manager = ManagerBuilder.Build();
                manager.Approval(application);
            }
        }
    }
}
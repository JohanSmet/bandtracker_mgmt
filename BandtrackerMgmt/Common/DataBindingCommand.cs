using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace BandtrackerMgmt
{
    public partial class BaseCommand : ICommand
    {
        public BaseCommand(bool p_can_execute)
        {
            m_can_execute = p_can_execute;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void ICommand.Execute(object parameter)
        {
            DoExecute(parameter);
        }

        // private parts
        protected virtual void DoExecute(object parameter)
        {
        }

        // properties
        public bool CanExecute
        {
            get { return m_can_execute; }
            set
            {
                if (m_can_execute != value)
                {
                    m_can_execute = value;

                    // raise event
                    EventHandler canExecuteChanged = CanExecuteChanged;
                    if (canExecuteChanged != null)
                        canExecuteChanged(this, EventArgs.Empty);
                }
            }
        }

        // events
        public event EventHandler CanExecuteChanged;

        // member variables
        private bool m_can_execute;
    }

    public class SimpleCommand : BaseCommand
    {
        // interface
        public SimpleCommand(Action p_action, bool p_can_execute = true)
            : base(p_can_execute)
        {
            m_action = p_action;
        }

        // private parts
        protected override void DoExecute(object parameter)
        {
            if (m_action != null)
                m_action();
        }

        // member variables
        private readonly Action m_action;
    }

    public class ParamCommand<T> : BaseCommand
    {
        // interface
        public ParamCommand(Action<T> p_action, bool p_can_execute = true)
            : base(p_can_execute)
        {
            m_action = p_action;
        }

        // private parts
        protected override void DoExecute(object parameter)
        {
            if (m_action != null)
                m_action((T)parameter);
        }

        // member variables
        private readonly Action<T> m_action;
    }

    public class CommandGroup
    {
        // command management
        public SimpleCommand AddCommand(SimpleCommand p_command)
        {
            m_commands.Add(p_command);
            return p_command;
        }

        public ParamCommand<T> AddCommand<T>(ParamCommand<T> p_command)
        {
            m_commands.Add(p_command);
            return p_command;
        }

        // change CanExecute for all commands
        public void SetCanExecute(bool p_can_execute)
        {
            foreach (var f_cmd in m_commands)
            {
                f_cmd.CanExecute = p_can_execute;
            }
        }

        public void ToggleCanExecute()
        {
            foreach (var f_cmd in m_commands)
            {
                f_cmd.CanExecute = !f_cmd.CanExecute;
            }
        }

        // member variables
        private List<BaseCommand> m_commands = new List<BaseCommand>();
    }
}

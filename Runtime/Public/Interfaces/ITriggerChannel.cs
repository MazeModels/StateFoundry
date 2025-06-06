using System;

namespace Maze.StateFoundry
{
    public interface ITriggerChannel
    {
        public void Send<TTrigger>(TTrigger trigger) where TTrigger : struct, ITrigger;
        public void Listen<TTrigger>(Action<TTrigger> callback) where TTrigger : struct, ITrigger;
    }
}
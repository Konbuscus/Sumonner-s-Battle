using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sumonner_s_Battle.StateManager
{

    public interface IStateManager
    {
        GameState CurrentState { get; }

        event EventHandler StateChanged;

        void PushState(GameState state, PlayerIndex? index);
        void ChangeState(GameState state, PlayerIndex? index);
        void PopState();
        bool ContainsState(GameState state);
    }


    public class GameStateManager : GameComponent, IStateManager
    {
        #region Champs
        private readonly Stack<GameState> gameStates = new Stack<GameState>();

        private const int startDrawOrder = 5000;
        private const int drawOrderInc = 50;
        private int drawOrder;
        #endregion


        #region Gestion d'event
        public event EventHandler StateChanged;
        #endregion


        #region Propriétés
        public GameState CurrentState
        {
            get { return gameStates.Peek(); }
        }
        #endregion;


        #region Constrcteurs 
        public GameStateManager(Game game):base(game)
        {
            Game.Services.AddService(typeof(IStateManager), this);
        }
        #endregion


        #region Méthodes

        public void PushState(GameState state, PlayerIndex? index)
        {
            drawOrder += drawOrderInc;
            AddState(state, index);
            OnStateChanged();
        }

        public void AddState(GameState state, PlayerIndex? index)
        {
            gameStates.Push(state);
            state.PlayerIndexInControl = index;
            Game.Components.Add(state);
            StateChanged += state.StateChanged;
        }

        public void ChangeState(GameState state, PlayerIndex? index)
        {
            while (gameStates.Count > 0) {

                RemoveState();
                drawOrder = startDrawOrder;
                state.DrawOrder = drawOrder; 
                drawOrder += drawOrderInc; 
                AddState(state, index); OnStateChanged();
            } 
        }


        public void PopState()
        {
            if(gameStates.Count != 0)
            {
                RemoveState();
                drawOrder -= drawOrderInc;
                OnStateChanged();
            }
        }

        public void RemoveState()
        {
            GameState state = gameStates.Peek();
            StateChanged -= state.StateChanged;
            Game.Components.Remove(state);
            gameStates.Pop();
        }

        public bool ContainsState(GameState state)
        {
            return gameStates.Contains(state);
        }

        protected internal virtual void OnStateChanged()
        {
            if(StateChanged != null)
            {
                StateChanged(this, null);
            }
        }

        #endregion
    }
}

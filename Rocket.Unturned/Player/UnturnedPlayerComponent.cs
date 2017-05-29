using UnityEngine;

namespace Rocket.Unturned.Player
{
    public class UnturnedPlayerComponent : MonoBehaviour
    {
        public UnturnedPlayer Player { get; private set; }

        private void Awake()
        {
            Player = UnturnedPlayer.FromPlayer(gameObject.transform.GetComponent<SDG.Unturned.Player>());
            DontDestroyOnLoad(transform.gameObject);
        }

        private void OnEnable()
        {
            Load();
        }

        private void OnDisable()
        {
            Unload();
        }

        protected virtual void Load()
        {

        }


        protected virtual void Unload()
        {

        }

    }
}
using UnityEngine;

public class GameSectors : MonoBehaviour
{
    public PlayerController m_Player;
    public Sector[] m_Sectors;

    private void Update()
    {
        foreach (Sector sector in m_Sectors)
        {
            bool isPlayerClose = sector.isPlayerClose(m_Player.transform.position);

            if (isPlayerClose != sector.isLoaded)
                sector.markDirty();

            if (sector.isDirty)
                if (isPlayerClose)
                    sector.LoadContent();
                else
                    sector.UnloadContent();

            sector.Clean();
        }
    }
}

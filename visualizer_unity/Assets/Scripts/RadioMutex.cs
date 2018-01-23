using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioMutex : MonoBehaviour {

    public BrowserCommunication comm;
    public int station;

    public void do_set_station() {
        try
        {
            comm.select_station(station);
        }
        catch(System.Exception e)
        {
            throw new System.Exception();
        }
        
    }

}

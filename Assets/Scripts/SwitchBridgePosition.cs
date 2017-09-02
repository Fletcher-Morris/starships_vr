using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchBridgePosition : MonoBehaviour
{
    public List<PositionRotation> positions;
    public int currentPosition = 1;

    private void Start()
    {
        SwitchPosition(1);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
			NextPosition ();
        }
    }

	public void NextPosition()
	{
		if(currentPosition + 1 <= positions.Count)
		{
			currentPosition++;
			SwitchPosition(currentPosition);
		}
		else
		{
			currentPosition = 1;
			SwitchPosition(currentPosition);
		}
	}

    public void SwitchPosition(int posId)
    {
        transform.position = positions[posId - 1].position;
        transform.eulerAngles = positions[posId - 1].rotation;
    }
}

[System.Serializable]
public class PositionRotation
{
    public Vector3 position;
    public Vector3 rotation;
}
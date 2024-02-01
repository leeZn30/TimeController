using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    Character character;

    private void Awake()
    {
        character = FindObjectOfType<Character>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(character.transform.position.x, character.transform.position.y, transform.position.z);
    }


}

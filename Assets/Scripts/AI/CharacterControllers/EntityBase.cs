using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityBase : MonoBehaviour {

    public abstract void OnDeath();
    public abstract void OnSpawn();
    public abstract void OnEntityHit();
    public abstract void Move();
    public abstract void ChooseAction();

}

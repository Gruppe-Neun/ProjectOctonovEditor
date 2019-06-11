using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable {
    void Interact();
}

public interface IDamageable {
    void TakeDamage(float damage);
}

public interface IDamageableEnemy : IDamageable {

}

public interface IDamageableFriendly : IDamageable {

}
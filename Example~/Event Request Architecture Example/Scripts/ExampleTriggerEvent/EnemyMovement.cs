using System.Collections;
using UnityEngine;

namespace Event_Request_Architecture.Example.Event_Request_Architecture_Example.Scripts.ExampleTriggerEvent
{
    public class EnemyMovement : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float distance;

        private IEnumerator Start()
        {
            var moveRight = Random.value > 0.5f;
            
            transform.position = new Vector3(moveRight ? distance : -distance, 0);

            var rightPosition = new Vector3(distance, 0);
            var leftPosition = new Vector3(-distance, 0);
            
            var targetPosition = new Vector3(!moveRight ? distance : -distance, 0);;

            while (true)
            {

                while (transform.position != targetPosition)
                {
                    transform.position =
                        Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                    yield return null;
                }

                if (transform.position == targetPosition)
                    targetPosition = targetPosition == rightPosition ? leftPosition : rightPosition;
                yield return null;
            }
        }
    }
}
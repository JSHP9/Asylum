using UnityEngine;

public class EnemySight : MonoBehaviour
{
    [Header("Sight Settings")]
    public Transform player; // 플레이어 타겟
    public float viewRadius = 15f; // 귀신이 볼 수 있는 최대 거리
    // [Range(0, 360)]은 Unity의 Attribute(특성)이라함, nspector에서 값을 입력할 때 슬라이더를 만들어줌
    // 이렇게 선언하면 인스펙터에서는 강제력이 생김, 코드는 아님
    [Range(0, 360)] public float viewAngle = 140f; // 귀신의 시야각, 전방 90도면 좌우 45도씩

    public LayerMask obstacleMask; // 벽, 문 등 시야를 가리는 장애물 레이어

    public bool CanSeePlayer()
    {
        // 플레이어와의 거리 잼
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > viewRadius)
        {
            return false; // 사야보다 멀리 있음
        }

        // 플레이어가 귀신의 시야각 안에 있는지 노말벡터로 각도 재서 구함
        Vector3 dirToPlayer = (player.position - transform.position).normalized; // 타겟을 향하는 방향

        // 시야각은 좌우(XZ 평면)만 계산하기 위해 높이(Y)는 무시한다.
        // (플레이어가 앉거나 계단에 있어도 시야각이 이상하게 커지는 것을 방지)
        dirToPlayer.y = 0f;
        dirToPlayer.Normalize(); // Y를 변경했으므로 다시 단위 벡터로 정규화
        float angleToPlayer = Vector3.Angle(transform.forward, dirToPlayer);

        // 시야각(viewAngle)이 왼쪽 45, 오른쪽 45해서 90인거라 45보다 크면 시야 밖에 있는거임
        if (angleToPlayer > viewAngle / 2f)
        {
            return false;
        }

        // 사이에 벽 있는지 확인
        // ai위치, 플레이어 각도, 플레이어 거리, 벽 레이어
        if (Physics.Raycast(transform.position, dirToPlayer, distanceToPlayer, obstacleMask))
        {
            return false; // 안보임
        }

        // 보임
        return true;
    }
}

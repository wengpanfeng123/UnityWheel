/*************************************************************************
 * Copyright  xicheng. All rights reserved.
 *------------------------------------------------------------------------
 * File     : PlayerController2D.cs
 * Author   : xicheng
 * Date     : 2025-09-17 09:53
 * Tips     : xicheng知识库
 * Description : 2D 平台跳跃角色完整实现
 *
 *一、渲染优化：1.使用途径。 2.非必要不使用半透明精灵，减少overdraw 3.动态批处理
 *二、物理优化：降低 2D 碰撞计算消耗。
 *   静态碰撞体合并：场景中多个静态平台（如地面、墙壁），用 “Composite Collider 2D” 合并成一个碰撞体，减少碰撞检测次数；
 *      操作：给父对象添加Composite Collider 2D和Rigidbody2D（Body Type 设为 Static），给子平台添加Box Collider 2D并勾选 “Used By Composite”；
 *   减少动态碰撞体数量：场景中动态对象（如敌人、道具）数量控制在 20 以内（移动端 10 以内），超出时用 “对象池” 复用。
 *三、内存优化：避免内存泄露
 *  1.动态加载精灵：用 “Addressables” 加载场景精灵，场景切换后释放未使用资源。
 *  2.卸载无用动画：角色死亡 / 场景切换后，销毁 Animator 组件或设置animator.enabled = false，避免后台播放消耗 CPU。
 */

using UnityEngine;
namespace Xicheng.Movement
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(CapsuleCollider2D))]
    public class PlayerController2D : MonoBehaviour
    {
        // 移动参数
        public float moveSpeed = 5f;
        public float jumpForce = 7f;
        public float gravityScale = 3f;

        // 碰撞检测参数
        public LayerMask groundLayer;
        public Transform groundCheck;
        public float groundCheckRadius = 0.2f;
        private bool isGrounded;
        private bool isJumping; // 标记是否在跳跃中

        // 组件引用
        private Rigidbody2D rb;
        private Animator animator;
        private CapsuleCollider2D collider2D;

        void Awake()
        {
            // 获取组件（避免Inspector赋值错误）
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            collider2D = GetComponent<CapsuleCollider2D>();

            // 初始化刚体参数
            rb.gravityScale = gravityScale;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete; // 角色用离散检测，性能好
        }

        void Update()
        {
            // 1. 地面检测（每帧更新）
            CheckGrounded();

            // 2. 玩家输入处理
            HandleInput();

            // 3. 动画状态同步
            UpdateAnimationState();
        }

        // 物理相关逻辑放在FixedUpdate（与物理更新同步）
        void FixedUpdate()
        {
            HandleMovement();
        }

        /// <summary>
        /// 地面检测：判断角色是否在地面
        /// </summary>
        private void CheckGrounded()
        {
            // 用OverlapCircle检测地面（参数：检测位置、半径、检测层）
            Collider2D[] groundColliders = Physics2D.OverlapCircleAll(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );

            // 只要检测到地面碰撞体，就视为在地面
            isGrounded = groundColliders.Length > 0;
            // 落地时重置跳跃标记
            if (isGrounded && isJumping)
            {
                isJumping = false;
            }
        }

        /// <summary>
        /// 输入处理：移动、跳跃
        /// </summary>
        private void HandleInput()
        {
            // 跳跃输入（仅在地面时可跳）
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                isJumping = true;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }

            // 二段跳（可选，根据需求开启）
            // if (Input.GetKeyDown(KeyCode.Space) && !isGrounded && !hasDoubleJumped)
            // {
            //     hasDoubleJumped = true;
            //     rb.velocity = new Vector2(rb.velocity.x, jumpForce * 0.8f); // 二段跳力度稍小
            // }
        }

        /// <summary>
        /// 移动处理：左右移动、角色翻转
        /// </summary>
        private void HandleMovement()
        {
            // 获取水平输入（-1：左，1：右，0：无输入）
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            if (horizontalInput != 0)
            {
                // 计算移动速度（仅水平方向，垂直方向由重力控制）
                Vector2 moveVelocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
                rb.velocity = moveVelocity;

                // 角色翻转：根据移动方向调整缩放X轴
                transform.localScale = new Vector3(
                    horizontalInput > 0 ? 1 : -1,
                    transform.localScale.y,
                    transform.localScale.z
                );
            }
        }

        /// <summary>
        /// 动画状态同步：根据角色状态更新动画
        /// </summary>
        private void UpdateAnimationState()
        {
            // 1. 跑步动画：有水平输入且在地面
            bool isRunning = Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f && isGrounded;
            animator.SetBool("IsRunning", isRunning);

            // 2. 跳跃动画：不在地面且向上运动
            bool isJumpingUp = !isGrounded && rb.velocity.y > 0.1f;
            animator.SetBool("IsJumpingUp", isJumpingUp);

            // 3. 下落动画：不在地面且向下运动
            bool isFalling = !isGrounded && rb.velocity.y < -0.1f;
            animator.SetBool("IsFalling", isFalling);

            // 4. 待机动画：在地面且无移动
            bool isIdle = isGrounded && !isRunning;
            animator.SetBool("IsIdle", isIdle);
        }

        /// <summary>
        /// Gizmos：在Scene视图绘制地面检测范围（调试用）
        /// </summary>
        void OnDrawGizmosSelected()
        {
            if (groundCheck != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
            }
        }

        /// <summary>
        /// 碰撞回调：处理与平台的交互（如穿过平台）
        /// </summary>
        void OnCollisionEnter2D(Collision2D collision)
        {
            // 示例：穿过平台（按下下键时可从平台下落）
            if (collision.gameObject.CompareTag("Platform") && Input.GetAxisRaw("Vertical") < -0.1f)
            {
                // 暂时禁用与平台的碰撞，实现下落
                Physics2D.IgnoreCollision(collider2D, collision.collider, true);
                // 1秒后恢复碰撞（避免一直穿模）
                Invoke(nameof(RestorePlatformCollision), 1f);
            }
        }

        /// <summary>
        /// 恢复与平台的碰撞
        /// </summary>
        private void RestorePlatformCollision()
        {
            Collider2D[] platformColliders = Physics2D.OverlapCircleAll(
                transform.position,
                1f,
                LayerMask.GetMask("Platform")
            );
            foreach (var collider in platformColliders)
            {
                Physics2D.IgnoreCollision(collider2D, collider, false);
            }
        }
    }
}
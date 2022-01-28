using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using QFramework;
using UnityEngine;
using UnityEngine.SceneManagement;

//#define ENABLE_ANIM

public class GlobalStateMachine : Singleton<GlobalStateMachine>
{
    public static bool DisableAnimation = false;

    private bool flag1 = false;
    private bool flag2 = false;

    #region Properties

    //basic properties
    private int cur_state = 0;
    private int cur_sub_state = 0;

    public int CurState => cur_state;
    public int CurSubState => cur_sub_state;

    private List<Level> levels = new List<Level>();
    public List<Level> Levels => levels;

    //other properties
    private int max_state = 3; //total level count

    private int ADPressedCount = 0;
    private int SpacePressedCount = 0;

    public float UIheight = 60f;

    #endregion


    #region feature functions

    /// <summary>
    /// check if current state already completed, then jump to the next state if true
    /// </summary>
    public void CheckState()
    {
        //Debug.Log(cur_sub_state);
        if (cur_sub_state >= Levels[cur_state].parts.Count)
        {
            return;
        }

        var conditionFunction = Levels[cur_state].parts[cur_sub_state].completeCondition;
        if (conditionFunction == null)
        {
            throw new UnityException("condition functions could not be null");
            return;
        }

        if (conditionFunction.Invoke())
        {
            Continue();
        }
    }

    /// <summary>
    /// jump to the next state
    /// </summary>
    private void Continue()
    {
        if (cur_state > max_state)
        {
            Debug.LogError("游戏结束");
            return;
        }

        if (!levels[cur_state].JumpToNextPart(ref cur_sub_state))
        {
            Debug.Log("<color=red>Complete current level :: </color>" + cur_state);
            ++cur_state;
            cur_sub_state = 0;
            Continue();
        }
    }

    #endregion


    #region initialize functions

    //**********************************************************************//
    //-----------------------------  Level 1  
    //**********************************************************************//
    private void InitLevel1()
    {
        //image id:
        //城镇A-城镇B    11
        //，人群-人群    12
        //，年兽-年兽    13
        //，爆竹         14
        //，景观-篝火    15
        GameObject _cheng_zhen = GameObject.Find("Nulll");
        GameObject _ren_qun = GameObject.Find("Nulll");
        GameObject _gou_huo = GameObject.Find("Nulll");
        GameObject _bao_zhu = GameObject.Find("Nulll");
        GameObject _nian_shou = GameObject.Find("Nulll");

        Level newLevel = new Level();
        Part tmpPart;

        //var sequenecNode = new SequenceNode();
        //part 0
        tmpPart = new Part(new DelayAction[]
            {
                //DelayAction.Allocate(1.0f, () => Debug.Log("延时 1 秒" + DateTime.Now)),
                //log
                //DelayAction.Allocate(0.5f,()=> CameraController.ChangeToView2d()), 
                //DelayAction.Allocate(1f,()=> Debug.Log("Hello")), 
                DelayAction.Allocate(0.5f, delegate { CameraController.ChangeToView2d(); }),


                DelayAction.Allocate(0f, () => DialogueMgr.Instance.Log("旧时，戏言山北怪谈湖南有一奇镇，名”四面“。", 0, UIheight)), //120
                DelayAction.Allocate(0f, () => CameraController.PressADcallBack = () => ADPressedCount++),
                DelayAction.Allocate(3f, () =>
                {
                    Main.InputActionEditable = false;
                    Main.InputActionAliveAD = true;
                    Main.InputActionAliveSpace = false;

                    // item 初始坐标
                    _cheng_zhen = Resources.Load<GameObject>("Level1/_cheng_zhen").Instantiate();
                    _cheng_zhen.transform.SetParent(EntitiesMgr.Instance.transform);
                    _cheng_zhen.transform.localPosition = new Vector3(-1 * SpawnMgr.gap, 0, -2 * SpawnMgr.gap);
                    _cheng_zhen.transform.localScale = new Vector3(5f, 5f, 5f);


                    _ren_qun = Resources.Load<GameObject>("Level1/_ren_qun").Instantiate();
                    _ren_qun.transform.SetParent(EntitiesMgr.Instance.transform);
                    _ren_qun.transform.localPosition = new Vector3(1 * SpawnMgr.gap, 0, -2 * SpawnMgr.gap);

                    _gou_huo = Resources.Load<GameObject>("Level1/_gou_huo").Instantiate();
                    _gou_huo.transform.SetParent(EntitiesMgr.Instance.transform);
                    _gou_huo.transform.localPosition = new Vector3(2 * SpawnMgr.gap, 0, 2 * SpawnMgr.gap);

                    _bao_zhu = Resources.Load<GameObject>("Level1/_bao_zhu").Instantiate();
                    _bao_zhu.transform.SetParent(EntitiesMgr.Instance.transform);
                    _bao_zhu.transform.localPosition = new Vector3(-2 * SpawnMgr.gap, 0, 2 * SpawnMgr.gap);
                    
                    EntitiesMgr.Instance.ScaleWithinRotation();
                }),

                //DelayAction.Allocate(0.5f, delegate { CameraController.ChangeToView2d();})
            })
            .AddCompleteCondition4self(() => { return true; });
        newLevel.parts.Add(tmpPart);

        //part 1
        tmpPart = new Part(new DelayAction[]
            {
                DelayAction.Allocate(0f, () => CameraController.ChangeToView2d()),
                DelayAction.Allocate(0f, () => DialogueMgr.Instance.Log("每冬末春初，此镇便万人空巷人声鼎沸，可谓热闹至极。", 0, UIheight)),
                DelayAction.Allocate(0.5f, delegate { SelectMgr.Instance.CatchEntityAlive = true; }),
            })
            .AddCompleteCondition4self(() =>
            {
                //Debug.Log("cur RotateCount = " + CameraController.RotateCount);
                //玩家按了几次A/D
                if (CameraController.RotateCount == 0 && ADPressedCount >= 6)
                {
                    Debug.Log("AD Pressed 6 count");
                    CameraController.PressADcallBack = null;
                    return true;
                }

                return false;
            });
        newLevel.parts.Add(tmpPart);

        //part 2
        tmpPart = new Part(new DelayAction[]
            {
                //2d view
                //rotate view
                DelayAction.Allocate(0f, () =>
                {
                    SelectMgr.Instance.CatchEntityAlive = false;

                    CameraController.viewing_state = ViewingState.V2d;
                    CameraController.RotateCount = 0;
                    Main.InputActionAliveAD = false;
                }),

                //年兽从天而降 prefab path
                DelayAction.Allocate(0.5f, () =>
                {
                    _nian_shou = Resources.Load<GameObject>("Level1/_nian_shou").Instantiate();
                    _nian_shou.transform.SetParent(EntitiesMgr.Instance.transform);
                    _nian_shou.transform.localPosition = new Vector3(-1 * SpawnMgr.gap, 10, 0 * SpawnMgr.gap);
                    _nian_shou.transform
                        .DOLocalMove(new Vector3(-1 * SpawnMgr.gap, 0, 0 * SpawnMgr.gap), 1f)
                        .SetEase(Ease.Flash);
                    EntitiesMgr.Instance.ScaleWithinRotation();
                }),
#if ENABLE_ANIM
            DelayAction.Allocate(3f, () => DialogueMgr.Instance.Log("却不想触怒异兽，引来祸患。", 0, UIheight )),
            DelayAction.Allocate(4f, () => DialogueMgr.Instance.Log("吾名为‘年’，久受汝等扰梦，今特来一绝后患", 0, UIheight )),
            
            /*
                1.”娘，这只狗狗在说什么？“
                2.”傻孩子，它说要吃了咱们呢。“
                3.”什么？我没有——“
             */
            //人群变为惊恐状
            DelayAction.Allocate(1f, () =>
            {
                var go = EntitiesMgr.Instance.GetGameObjectByImageId(12);
                float x, y;
                x = (int) (go.transform.position.x / SpawnMgr.gap);
                y = (int) (go.transform.position.z / SpawnMgr.gap);
                go.DestroySelf();
                // Prefab
                GameObject _renqun = Resources.Load<GameObject>("Level1/_ren_qun2").Instantiate();
                _renqun.transform.SetParent(EntitiesMgr.Instance.transform);
                _renqun.transform.localPosition = new Vector3(x * SpawnMgr.gap, 0, y * SpawnMgr.gap);
                _renqun.GetComponent<Entity>().imageId = 12;
                EntitiesMgr.Instance.ScaleWithinRotation();
            } ),
            DelayAction.Allocate(2f, () => DialogueMgr.Instance.Log("“娘，这只狗狗在说什么？”", 0, UIheight )),
            DelayAction.Allocate(2f, () => DialogueMgr.Instance.Log("“傻孩子，它说要吃了咱们呢。”", 0, UIheight )),
            DelayAction.Allocate(2f, () => DialogueMgr.Instance.Log("“什么？我没有——！”", 0, UIheight )),
            
            /*
                1.”大伙！快到房子后面去！这只怪物要生吃我们！“
                2.”别乱说！我不是——“
                3.”跑啊！“
             */
            //年兽向人群方向靠近，却因为房子挡住（有碰撞），过不去
            DelayAction.Allocate(2f, () =>
            {
                var tmpGO1 = EntitiesMgr.Instance.GetGameObjectByImageId(13);
                tmpGO1.transform.DOPunchPosition(tmpGO1.transform.position, 0.5f);
            }),

            DelayAction.Allocate(2f, () => DialogueMgr.Instance.Log("大伙！快到房子后面去！这只怪物要生吃我们！", 0, UIheight )),
            DelayAction.Allocate(2f, () => DialogueMgr.Instance.Log("糟了！人类之力怎能与精怪抗衡？那年兽竟一动不动！", 0, UIheight )),
            //DelayAction.Allocate(0f, () => DialogueMgr.Instance.Log("1.”大伙！快到房子后面去！这只怪物要生吃我们！“", 0, 120 )),
            //DelayAction.Allocate(0f, () => DialogueMgr.Instance.Log("2.”别乱说！我不是——“", 0, 120 )),
            //DelayAction.Allocate(0f, () => DialogueMgr.Instance.Log("3.”跑啊！“", 0, 120 )),

            /*
                1.这时候镇子里最勇敢的小伙子大喊——
                2.“保卫四方镇！”"
             */
            //年兽和房子和人群抖动
            DelayAction.Allocate(2f, () =>
            {
                //Debug.Log("年兽和房子和人群抖动");
                
                var tmpGO1 = EntitiesMgr.Instance.GetGameObjectByImageId(11);
                tmpGO1.transform.DOPunchPosition(tmpGO1.transform.position, 0.5f);
                var tmpGO2 = EntitiesMgr.Instance.GetGameObjectByImageId(12);
                tmpGO2.transform.DOPunchPosition(tmpGO2.transform.position, 0.5f);
                var tmpGO3 = EntitiesMgr.Instance.GetGameObjectByImageId(13);
                tmpGO3.transform.DOPunchPosition(tmpGO3.transform.position, 0.5f);
                //EntitiesMgr.Instance.GetGameObjectByImageId(12).transform.DOPunchRotation(Vector3.zero, 0.5f);
                //EntitiesMgr.Instance.GetGameObjectByImageId(13).transform.DOPunchRotation(Vector3.zero, 0.5f);
            } ),
#endif
                DelayAction.Allocate(3f, () =>
                {
                    DialogueMgr.Instance.Log("或许，还有力量可凌驾于神魔之上，不过要从哪寻得此力呢？", 0, UIheight);

                    //
                    Main.InputActionAliveAD = true;
                    SelectMgr.Instance.CatchEntityAlive = false;

                    //TODO: 提示玩家按下A\D切换到蓝2d面
                    //Main.InputActionAliveAD = true;
                    Debug.Log("part 2 end");
                }),

                /*
                    人类之力怎能与精怪抗衡？那年兽竟一动不动！
                 */
                //DelayAction.Allocate(0f, () => DialogueMgr.Instance.Log("人类之力怎能与精怪抗衡？那年兽竟一动不动！", 0, 120 )),

                //end
                //或许，还有力量可凌驾于神魔之上，不过要从哪寻得此力呢？
                //DelayAction.Allocate(0f, () => DialogueMgr.Instance.Log("或许，还有力量可凌驾于神魔之上，不过要从哪寻得此力呢？", 0, 120 )),
            })
            .AddCompleteCondition4self(() =>
            {
                //check item position
                //房子 22 -> 17
                if (EntitiesMgr.Instance.CheckCoordByImageId(11, -1, -1))
                {
                    return true;
                }

                return false;
            });
        newLevel.parts.Add(tmpPart);

        //part 3
        tmpPart = new Part(new DelayAction[]
            {
                //"怪物消失了！“
                DelayAction.Allocate(0f, () => CameraController.RotateCount = 1),
                DelayAction.Allocate(3f, () => DialogueMgr.Instance.Log("怪物消失了！", 0, UIheight)),
                DelayAction.Allocate(3f, () =>
                {
                    Main.InputActionAliveSpace = true;
                    DialogueMgr.Instance.Log("不行，这只是缓兵之计！", 0, UIheight);
                    CameraController.PressSpacecallBack = () => SpacePressedCount++;
                }),
            })
            .AddCompleteCondition4self(() =>
            {
                //切换到蓝2d面
                if (CameraController.RotateCount == 0)
                {
                    return true;
                }

                return false;
            });
        newLevel.parts.Add(tmpPart);

        //part 4
        tmpPart = new Part(new DelayAction[]
            {
                /*
                 * “啊啊啊啊啊”
                    （”不可！四面镇自古四面一体，我等怎可一面苟活？“）
                */
                //DelayAction.Allocate(0f, () => DialogueMgr.Instance.Log("“啊啊啊啊啊”", 0, 120 )),
                //DelayAction.Allocate(0f, () => DialogueMgr.Instance.Log(" （”不可！四面镇自古四面一体，我等怎可一面苟活？“）", 0, 120 )),

                /*
                    1.想想其他办法吧！这时候人群中最睿智的老者道——
                    2.“年兽怕响声，快点燃鞭炮！”
                    3.“怎么办？够不到！年兽快过来了！”
                    4.”高于神明的存在啊！请救救我们！“
                 */
                //DelayAction.Allocate(0f, () => DialogueMgr.Instance.Log("1.想想其他办法吧！这时候人群中最睿智的老者道——", 0, 120 )),
                //DelayAction.Allocate(0f, () => DialogueMgr.Instance.Log("2.“年兽怕响声，快点燃鞭炮！”", 0, 120 )),
                //DelayAction.Allocate(0f, () => DialogueMgr.Instance.Log("3.“怎么办？够不到！年兽快过来了！”", 0, 120 )),

                /*
                 * "1.“必须要想办法彻底赶走怪物才行！”
                    2.“那怪物怕响声，快点燃鞭炮！””"
                 */
#if ENABLE_ANIM
            DelayAction.Allocate(1f, () => DialogueMgr.Instance.Log("必须要想办法彻底赶走怪物才行！", 0, UIheight )),
            DelayAction.Allocate(3f, () => DialogueMgr.Instance.Log("那怪物怕响声，快点燃鞭炮！", 0, UIheight )),
#endif
            })
            .AddCompleteCondition4self(() =>
            {
                //Debug.Log(SpacePressedCount);
                //按下空格键
                if (SpacePressedCount >= 1)
                {
                    return true;
                }

                return false;
            });
        newLevel.parts.Add(tmpPart);

        //part 5
        tmpPart = new Part()
            .AddCompleteCondition4self(() =>
            {
                //篝火 -> 25 || 篝火 ->10
                if (EntitiesMgr.Instance.CheckCoordByImageId(15, -1, 2))
                {
                    return true;
                }

                return false;
            });

        //part 6:清除并进入下一关
        tmpPart = new Part(new DelayAction[]
            {
                //年兽飞出，人群欢呼
                DelayAction.Allocate(0f, () =>
                {
                    EntitiesMgr.Instance.GetGameObjectByImageId(13).transform.DOLocalMoveY(10, 0.5f);
                }),
                DelayAction.Allocate(0f, () => { Debug.Log("第一关完成！"); }),

                DelayAction.Allocate(0f, () => { EntitiesMgr.Instance.ClearAll(); }),
            })
            .AddCompleteCondition4self(() =>
            {
                if (Input.GetKey(KeyCode.N))
                {
                    return true;
                }

                return false;
            });


        //_cheng_zhen.DestroyAllChildren();
        //_gou_huo.DestroyAllChildren();
        //_nian_shou.DestroyAllChildren();
        //_ren_qun.DestroyAllChildren();
        //_bao_zhu.DestroyAllChildren();


        newLevel.parts.Add(tmpPart);


        levels.Add(newLevel);
    }


    //**********************************************************************//
    //-----------------------------  Level 2  
    //**********************************************************************//
    private void InitLevel2()
    {
        Level newLevel = new Level();
        Part tmpPart = new Part();

        /*
         * ID:
         * 沉香：21
         * 云A：22
         * 云B：23
         * 沉香母亲：24
         * 狗：25
         * 没洞的山：26
         * 有洞的山：27
         * 瀑布和锄头：28
         * 种子：29
         * 藤曼：30
         * 愚公：31
         * 桥：32
         * 二郎：34
         * 左边房子：51
         * 右边房子：52
         *
         * 
         * ======待加
         * 愚公拿起锄头
         * 愚公放下锄头
         * 
         */
        //init temporary parts
        //part 0
        tmpPart = new Part(new DelayAction[]
            {
                //DelayAction.Allocate(0f,()=> GameObject.Find("Entities").DestroyAllChildren()),

                DelayAction.Allocate(0.5f, delegate { CameraController.ChangeToView2d(); }),
                //DelayAction.Allocate(0.5f, () => CameraController.PressADcallBack = () => ADPressedCount++ ),
                DelayAction.Allocate(1f, () =>
                {
                    GameObject.Find("Entities").DestroyAllChildren();

                    Main.InputActionEditable = true; //allow to enable all input action when set InputActionAlive value;
                    Main.InputActionAliveAD = true;
                    Main.InputActionAliveSpace = true;


                    //GameObject backimg=GameObject.Find("BgCylinder");

                    //GameObject.Find("BgCylinder").GetComponent<MeshRenderer>().material=Resources.Load<Material>("Level2/bgbg2");


                    //TODO: init item position
                    GameObject _chenXiang = Resources.Load<GameObject>("Level2/_ChenXiang").Instantiate();
                    _chenXiang.transform.SetParent(EntitiesMgr.Instance.transform);
                    _chenXiang.transform.localPosition = new Vector3(-1 * SpawnMgr.gap, 0, 2 * SpawnMgr.gap);

                    //51
                    GameObject _build1 = Resources.Load<GameObject>("Level2/_build2").Instantiate();
                    _build1.transform.SetParent(EntitiesMgr.Instance.transform);
                    _build1.transform.localPosition = new Vector3(-1 * SpawnMgr.gap, 0, 0 * SpawnMgr.gap);
                    //52
                    GameObject _build2 = Resources.Load<GameObject>("Level2/_build2").Instantiate();
                    _build2.transform.SetParent(EntitiesMgr.Instance.transform);
                    _build2.transform.localPosition = new Vector3(2 * SpawnMgr.gap, 0, -2 * SpawnMgr.gap);

                    GameObject _seed = Resources.Load<GameObject>("Level2/_Seed").Instantiate();
                    _seed.transform.SetParent(EntitiesMgr.Instance.transform);
                    _seed.transform.localPosition = new Vector3(-2 * SpawnMgr.gap, 0, 0 * SpawnMgr.gap);

                    GameObject _yuGong = Resources.Load<GameObject>("Level2/_YuGong").Instantiate();
                    _yuGong.transform.SetParent(EntitiesMgr.Instance.transform);
                    _yuGong.transform.localPosition = new Vector3(-2 * SpawnMgr.gap, 0, 1 * SpawnMgr.gap);

                    GameObject _bridge = Resources.Load<GameObject>("Level2/_bridge").Instantiate();
                    _bridge.transform.SetParent(EntitiesMgr.Instance.transform);
                    _bridge.transform.localPosition = new Vector3(0 * SpawnMgr.gap, 0, 2 * SpawnMgr.gap);


                    //河流左边
                    _build1.GetComponent<Entity>().imageId = 51;
                    //右边
                    _build2.GetComponent<Entity>().imageId = 52;

                    //init blockinng position
                    GameObject block1 = Resources.Load<GameObject>("Level2/_Block").Instantiate();
                    block1.transform.SetParent(EntitiesMgr.Instance.transform);
                    block1.transform.localPosition = new Vector3(-1 * SpawnMgr.gap, 0, -2 * SpawnMgr.gap);

                    GameObject block2 = Resources.Load<GameObject>("Level2/_Block").Instantiate();
                    block2.transform.SetParent(EntitiesMgr.Instance.transform);
                    block2.transform.localPosition = new Vector3(-1 * SpawnMgr.gap, 0, -1 * SpawnMgr.gap);

                    GameObject block3 = Resources.Load<GameObject>("Level2/_Block").Instantiate();
                    block3.transform.SetParent(EntitiesMgr.Instance.transform);
                    block3.transform.localPosition = new Vector3(0 * SpawnMgr.gap, 0, -1 * SpawnMgr.gap);

                    GameObject block4 = Resources.Load<GameObject>("Level2/_Block").Instantiate();
                    block4.transform.SetParent(EntitiesMgr.Instance.transform);
                    block4.transform.localPosition = new Vector3(0 * SpawnMgr.gap, 0, 0 * SpawnMgr.gap);

                    GameObject block5 = Resources.Load<GameObject>("Level2/_Block").Instantiate();
                    block5.transform.SetParent(EntitiesMgr.Instance.transform);
                    block5.transform.localPosition = new Vector3(0 * SpawnMgr.gap, 0, 1 * SpawnMgr.gap);

                    EntitiesMgr.Instance.ScaleWithinRotation();
                }),

                //DelayAction.Allocate(0.5f, delegate { CameraController.ChangeToView2d();}),
                DelayAction.Allocate(0.5f, delegate { SelectMgr.Instance.CatchEntityAlive = true; }),
            })
            .AddCompleteCondition4self(() => { return true; });
        newLevel.parts.Add(tmpPart);


        //part 2-1
        tmpPart = new Part(new DelayAction[]
            {
                //part 1 outro
                /*
            *"“开山斧，两刃刀，银弹金弓；升天帽，蹬云履，腾云驾雾——”
    ”娘！我不想再听什么二郎神的故事了，我想去外面玩。“
    ”你这孩子，二郎神可是大英雄，欸——“
    
    （沉香跑过桥）
    
    “沉香！跑慢点！”
    （三圣母上桥）
    ”娘，快过——啊！！！“
    ”娘！！！“
    "
            * 
            */
                //to 2d view
                DelayAction.Allocate(0.5f, delegate { CameraController.ChangeToView2d(); }),
                DelayAction.Allocate(0.5f, delegate { CameraController.RotateCount = 0; }),
//#if ENABLE_ANIM
                //dialog
#if ENABLE_ANIM
                DelayAction.Allocate(1f, delegate { DialogueMgr.Instance.Log("开山斧，两刃刀，银弹金弓；升天帽，蹬云履，腾云驾雾——", 0, 120); }),
                DelayAction.Allocate(3f, delegate { DialogueMgr.Instance.Log("娘！我不想再听什么二郎神的故事了，我想去外面玩。", 0, 120); }),
                DelayAction.Allocate(3f, delegate { DialogueMgr.Instance.Log("你这孩子，二郎神可是大英雄，唉——", 0, 120); }),
#endif
                // animation : 沉香跑过桥 end position
                DelayAction.Allocate(3f, delegate
                {
                    EntitiesMgr.Instance.GetGameObjectByImageId(21).transform.DOLocalMove(new Vector3(
                        0 * SpawnMgr.gap,
                        1.5f,
                        2 * SpawnMgr.gap
                    ), 0.5f).OnComplete(() =>
                    {
                        EntitiesMgr.Instance.GetGameObjectByImageId(21).transform.DOLocalMove(new Vector3(
                            1 * SpawnMgr.gap,
                            0,
                            2 * SpawnMgr.gap
                        ), 0.5f);
                    });
                }),
#if ENABLE_ANIM
                DelayAction.Allocate(3f, delegate { DialogueMgr.Instance.Log("沉香！跑慢点！", 0, 120); }),
#endif
                //animation : 三圣母上桥
                DelayAction.Allocate(3f, delegate
                {
                    //出厂
                    GameObject _sanShengMu = Resources.Load<GameObject>("Level2/_Cmama").Instantiate();
                    _sanShengMu.transform.SetParent(EntitiesMgr.Instance.transform);
                    _sanShengMu.transform.localPosition = new Vector3(-1 * SpawnMgr.gap, 0, 2 * SpawnMgr.gap);
                    EntitiesMgr.Instance.ScaleWithinRotation();
                    //向上
                    _sanShengMu.transform.DOLocalMove(new Vector3(0 * SpawnMgr.gap, 1.5f, 2 * SpawnMgr.gap), 0.5f);
                }),

                //anmation : 大山，瀑布，斧头自天儿下
                DelayAction.Allocate(3f, delegate
                {
                    GameObject mountain = Resources.Load<GameObject>("Level2/_Mountain").Instantiate();
                    mountain.transform.SetParent(EntitiesMgr.Instance.transform);
                    mountain.transform.localPosition = new Vector3(-1 * SpawnMgr.gap, 10, 2 * SpawnMgr.gap);

                    GameObject puBu = Resources.Load<GameObject>("Level2/_Pu_Chu").Instantiate();
                    puBu.transform.SetParent(EntitiesMgr.Instance.transform);
                    puBu.transform.localPosition = new Vector3(0 * SpawnMgr.gap, 10, 2 * SpawnMgr.gap);

                    GameObject fuTou = Resources.Load<GameObject>("Level2/_fu_tou").Instantiate();
                    fuTou.transform.SetParent(EntitiesMgr.Instance.transform);
                    fuTou.transform.localPosition = new Vector3(2 * SpawnMgr.gap, 10, 2 * SpawnMgr.gap);

                    EntitiesMgr.Instance.ScaleWithinRotation();

                    mountain.transform.DOLocalMoveY(0, 0.5f);
                    puBu.transform.DOLocalMoveY(0, 0.5f);
                    fuTou.transform.DOLocalMoveY(0, 0.5f);

                    EntitiesMgr.Instance.GetGameObjectByImageId(32).DestroySelf();
                    EntitiesMgr.Instance.GetGameObjectByImageId(24).DestroySelf();
                }),
#if ENABLE_ANIM
                DelayAction.Allocate(3f, delegate { DialogueMgr.Instance.Log("娘！！！", 0, 120); }),


                DelayAction.Allocate(3f,
                    delegate { DialogueMgr.Instance.Log("四面镇有江横千丈，阻两岸，扰民生，幸有郎君神赐桥以通两岸。", 0, 120); }),
                DelayAction.Allocate(3f,
                    delegate { DialogueMgr.Instance.Log("可天无不变风云，有山突降，毁桥隔岸。自此以后，两岸隔绝，互不相知。恍若双界，日月有异。", 0, 120); }),
#endif
                //rotate view to 【黄2d】
                DelayAction.Allocate(3f, delegate { CameraController.RotateCount = 1; }),
#if ENABLE_ANIM
                DelayAction.Allocate(3f,
                    delegate { DialogueMgr.Instance.Log("一岸五柳者，年且十九，面山而居。厌绝尘网，决心过山寻得桃花源。", 0, 120); }),
                DelayAction.Allocate(3f,
                    delegate
                    {
                        DialogueMgr.Instance.Log("欸！姓陶的小子，别傻了，没见那山上的斧头吗？那是二郎神降下来的，他妹妹跟凡人跑了，人家一怒之下才降山掷斧，你要过山？你就不怕神谴啊？",
                            0, 120);
                    }),
                DelayAction.Allocate(3f,
                    delegate { DialogueMgr.Instance.Log("去他妈的神力，神跟我过不去，我就跟他过不去，我偏要过山。", 0, 120); }),
                DelayAction.Allocate(3f,
                    delegate { DialogueMgr.Instance.Log("哟，得嘞，你还真是个榆木脑袋，你倒是说说，你要怎么移山？", 0, 120); }),
                DelayAction.Allocate(3f, delegate { DialogueMgr.Instance.Log("我.......我一定有办法的！", 0, 120); }),
#endif
                
                //animation  : 《二郎神浮在空中》
                DelayAction.Allocate(3f, delegate
                {
                    GameObject erLang = Resources.Load<GameObject>("Level2/_erLang").Instantiate();
                    erLang.transform.SetParent(EntitiesMgr.Instance.transform);
                    erLang.transform.localPosition = new Vector3(0 * SpawnMgr.gap, 10, 1 * SpawnMgr.gap);
                    EntitiesMgr.Instance.ScaleWithinRotation();
                    EntitiesMgr.Instance.GetGameObjectByImageId(34).transform.DOLocalMoveY(3, 0.5f);
                }),
                
#if ENABLE_ANIM
                DelayAction.Allocate(3f, delegate { DialogueMgr.Instance.Log("沉香。", 0, 120); }),
                DelayAction.Allocate(3f, delegate { DialogueMgr.Instance.Log("啊！你是谁？", 0, 120); }),
                DelayAction.Allocate(3f,
                    delegate { DialogueMgr.Instance.Log("我是二郎神。你的母亲三圣母触犯天条，现已被我镇压于山下，你母子将永世不可相见。", 0, 120); }),
                DelayAction.Allocate(3f, delegate { DialogueMgr.Instance.Log("什么？我要我娘！把我娘还给我！", 0, 120); }),
#endif
                //animation  : 《二郎神飞走》
                DelayAction.Allocate(3f,
                    delegate { EntitiesMgr.Instance.GetGameObjectByImageId(34).transform.DOLocalMoveY(10, 0.5f); }),
#if ENABLE_ANIM
                DelayAction.Allocate(3f, delegate { DialogueMgr.Instance.Log("去你的二郎神，我就算触犯天条也要找到娘。", 0, 120); }),
                DelayAction.Allocate(3f, delegate { DialogueMgr.Instance.Log("可是，这么一座大山，前面还有瀑布，要怎么办呢？", 0, 120); }),
                DelayAction.Allocate(3f, delegate { DialogueMgr.Instance.Log("如果有桥或者山洞就好了。", 0, 120); }),
#endif

                //rotate view to 【蓝2d】愚公
                DelayAction.Allocate(3f, delegate { CameraController.RotateCount = 3; }),
                
#if ENABLE_ANIM
                DelayAction.Allocate(3f, delegate { DialogueMgr.Instance.Log("一岸五柳者，年且十九，面山而居。厌绝尘网，决心过山寻得桃花源。", 0); }),
                DelayAction.Allocate(3f,
                    delegate
                    {
                        DialogueMgr.Instance.Log("欸！姓陶的小子，别傻了，没见那山上的斧头吗？那是二郎神降下来的，他妹妹跟凡人跑了，人家一怒之下才降山掷斧，你要过山？你就不怕神谴啊？",
                            0);
                    }),
                DelayAction.Allocate(3f, delegate { DialogueMgr.Instance.Log("去他妈的神力，神跟我过不去，我就跟他过不去，我偏要过山。", 0); }),
                DelayAction.Allocate(3f, delegate { DialogueMgr.Instance.Log("哟，得嘞，你还真是个榆木脑袋，你倒是说说，你要怎么移山？", 0); }),
                DelayAction.Allocate(3f, delegate { DialogueMgr.Instance.Log("我.......我一定有办法的！", 0); }),
                
#endif
            })
            .AddCompleteCondition4self(() => { return true; });
        newLevel.parts.Add(tmpPart);

        //part 2-2
        tmpPart = new Part(new DelayAction[]
            {
#if ENABLE_ANIM
                //DelayAction.Allocate(0.5f, delegate { DialogueMgr.Instance.Log("（沉香跑过桥）", 0, 120); }),
#endif
            })
            .AddCompleteCondition4self(() =>
            {
/*！！单击看全文！！
“四面镇有江横千丈，阻两岸，扰民生，幸有郎君神赐桥以通两岸。”
”可天无不变风云，有山突降，毁桥隔岸。自此以后，两岸隔绝，互不相知。恍若双界，日月有异。“
【1】”一岸五柳者，年且十九，面山而居。厌绝尘网，决心过山寻得桃花源。“
”欸！姓陶的小子，别傻了，没见那山上的斧头吗？那是二郎神降下来的，他妹妹跟凡人跑了，人家一怒之下才降山掷斧，你要过山？你就不怕神谴啊？“
“去他妈的神力，神跟我过不去，我就跟他过不去，我偏要过山。”
”哟，得嘞，你还真是个榆木脑袋，你倒是说说，你要怎么移山？“
”我.......我一定有办法的！“
【3】
”娘！娘！“
《二郎神浮在空中》
“沉香。”
”啊！你是谁？“
”我是二郎神。你的母亲三圣母触犯天条，现已被我镇压于山下，你母子将永世不可相见。“
“什么？我要我娘！把我娘还给我！”
《二郎神飞走》
“去你的二郎神，我就算触犯天条也要找到娘。”
“可是，这么一座大山，前面还有瀑布，要怎么办呢？”
“如果有桥或者山洞就好了。”*/

                /*在未通过的情况下，如果玩家做以下行为，则显示一句对应的旁白：
                    —移动斧子：（显示guidance4）
                    —移动山：“只可惜，这山怎样都移不出一条通路来”
                    —回到1面：见右边【旁白】-【1】
                    —回到3面：见右边【旁白】-【3】
                    —回到其他面：“桥没了，两岸互不相通”
                其中，【回到蓝2d】和【回到黄2d】为必要条件，无论其它行为有没有执行，只要玩家做了这俩行为，就判定通过2-1part*/
//                 if (/*移动斧子*/)
//                 {
//                     //显示guidance4
//                 }
//                 if (true/*移动山*/)
//                 {
// #if ENABLE_ANIM
//                     DialogueMgr.Instance.Log("只可惜，这山怎样都移不出一条通路来",0);
// #endif
//                 }

                return true;
            });
        newLevel.parts.Add(tmpPart);


        //part 2-3-1
//         tmpPart = new Part(new DelayAction[]
//             {
//                 /*
//                 "“这是......桥？”
//                     “莫非真有神力祝我？”"*/
// #if ENABLE_ANIM
//                 DelayAction.Allocate(0.5f, delegate { DialogueMgr.Instance.Log("这是......桥？", 0, 120); }),
//                 DelayAction.Allocate(0.5f, delegate { DialogueMgr.Instance.Log("莫非真有神力祝我？", 0, 120); }),
// #endif
//                 /*愚公过桥，来到沉香那一边
//                 画面转到【1面】
//                 愚公→12*/
//                 DelayAction.Allocate(0.5f, delegate { CameraController.RotateCount = 1; }),
//                 DelayAction.Allocate(0.5f,
//                     delegate
//                     {
//                         EntitiesMgr.Instance.GetGameObjectByImageId(31).transform.DOMove(new Vector3(
//                             SpawnMgr.gap*1, 0, SpawnMgr.gap*-1), 0.5f);
//                         
//                     }),
//                 
//                 
//                 /*
//                 ""有了！我以锄攻山，岁岁年年，必有一日寻得桃花源！"
//                 “什么？锄头？真是愚不可及，愚不可及啊！”
//                 "*/
// #if ENABLE_ANIM
//                 DelayAction.Allocate(delayTime: 0.5f, onDelayFinish: delegate { DialogueMgr.Instance.Log("有了，我以锄攻山，岁岁年年，必有一日寻得桃花源！", 0 , 120); } ),
//                 DelayAction.Allocate(delayTime: 0.5f, onDelayFinish: delegate { DialogueMgr.Instance.Log("什么？锄头？真是愚不可及，愚不可及啊1", 0, 120); }),
// #endif     
//
//             })
//             .AddCompleteCondition4self(() =>
//             {
//                 //房子移动到9 (0,-2)
//                 //房子的 image ID
//                 if (EntitiesMgr.Instance.CheckCoordByImageId(33, 0, -2) 
//                     && CameraController.RotateCount == 0
//                     && CameraController.viewing_state == ViewingState.V2d)
//                 {
//                     return true;
//                 }
//                 return false;
//             });
//         newLevel.parts.Add(tmpPart);


        //part 2-3-2
        tmpPart = new Part(new DelayAction[]
            {
                //愚公的图像变成【愚公拿着斧头】的图像
                DelayAction.Allocate(delayTime: 0.5f, onDelayFinish: delegate
                {
                    float x, y;
                    EntitiesMgr.Instance.GetCoordByImageId(31, out x, out y);
                    EntitiesMgr.Instance.GetGameObjectByImageId(31).DestroySelf();
                    //【愚公拿着斧头】 Prefab
                    GameObject _yuGong = Resources.Load<GameObject>("Level2/_YuGong_Fuzi").Instantiate();
                    _yuGong.transform.SetParent(EntitiesMgr.Instance.transform);
                    _yuGong.transform.localPosition = new Vector3(x * SpawnMgr.gap, 0, y * SpawnMgr.gap);
                    _yuGong.GetComponent<Entity>().imageId = 31;
                    EntitiesMgr.Instance.ScaleWithinRotation();
                }),
                //愚公移动到5
                DelayAction.Allocate(0.5f, delegate
                {
                    EntitiesMgr.Instance.GetGameObjectByImageId(31).transform.DOLocalMove(new Vector3(
                        SpawnMgr.gap * -2,
                        0,
                        SpawnMgr.gap * 2
                    ), 0.5f);
                }),

                //（如果山也占了5的格子的话，就先让山移动到7和9，再移动愚公）
                DelayAction.Allocate(delayTime: 0.5f, onDelayFinish: delegate
                {
                    if (EntitiesMgr.Instance.CheckCoordByImageId(26, -2, -2))
                    {
                        EntitiesMgr.Instance.GetGameObjectByImageId(26).transform.DOLocalMoveX(-1 * SpawnMgr.gap, 0.5f);
                    }

                    EntitiesMgr.Instance.GetGameObjectByImageId(31).transform.DOLocalMove(new Vector3(
                        -2 * SpawnMgr.gap,
                        0,
                        -2 * SpawnMgr.gap
                    ), 0.5f);
                }),
                //愚公物件上下震动一下表示它在挖山"
                //愚公的图像变成【愚公拿着斧头往下锄】的图像
                DelayAction.Allocate(delayTime: 0.5f, onDelayFinish: delegate
                {
                    float x, y;
                    EntitiesMgr.Instance.GetCoordByImageId(31, out x, out y);
                    EntitiesMgr.Instance.GetGameObjectByImageId(31).DestroySelf();
                    //【愚公拿着斧头】 Prefab
                    GameObject _yuGong = Resources.Load<GameObject>("Level2/_YuGong_Fuzi2").Instantiate();
                    _yuGong.transform.SetParent(EntitiesMgr.Instance.transform);
                    _yuGong.transform.localPosition = new Vector3(x * SpawnMgr.gap, 0, y * SpawnMgr.gap);
                    _yuGong.GetComponent<Entity>().imageId = 31;
                    EntitiesMgr.Instance.ScaleWithinRotation();
                }),
                DelayAction.Allocate(delayTime: 0.5f,
                    onDelayFinish: delegate { DialogueMgr.Instance.Log("一岸老者，年且九十，面山而居。以锄攻山，愚不可及，得名愚公。", 0, 120); }),

                //山被挖出洞
                DelayAction.Allocate(delayTime: 0.5f, onDelayFinish: delegate
                {
                    float x, y;
                    EntitiesMgr.Instance.GetCoordByImageId(26, out x, out y);
                    EntitiesMgr.Instance.GetGameObjectByImageId(26).DestroySelf();
                    //【愚公拿着斧头】 Prefab
                    GameObject _mountainO = Resources.Load<GameObject>("Level2/_MountainO").Instantiate();
                    _mountainO.transform.SetParent(EntitiesMgr.Instance.transform);
                    _mountainO.transform.localPosition = new Vector3(x * SpawnMgr.gap, 0, y * SpawnMgr.gap);
                    _mountainO.GetComponent<Entity>().imageId = 26;
                    EntitiesMgr.Instance.ScaleWithinRotation();
                }),
                /*画面转到【1面】
                愚公→19*/
                DelayAction.Allocate(delayTime: 0.5f, onDelayFinish: delegate
                {
                    CameraController.ChangeToView2d();
                    CameraController.RotateCount = 3;
  
                }),

                DelayAction.Allocate(delayTime: 1.5f, onDelayFinish: delegate
                {
                    EntitiesMgr.Instance.GetGameObjectByImageId(21).transform.DOLocalMove(new Vector3(
                        1 * SpawnMgr.gap,
                        0,
                        -1 * SpawnMgr.gap
                    ), 0.5f);
                }),
                DelayAction.Allocate(delayTime: 2.5f, onDelayFinish: delegate
                {
                    EntitiesMgr.Instance.GetGameObjectByImageId(31).transform.DOLocalMove(new Vector3(
                        1 * SpawnMgr.gap,
                        0,
                        2 * SpawnMgr.gap
                    ), 0.5f);
                }),
                DelayAction.Allocate(delayTime: 3.5f, onDelayFinish: delegate
                {
                    EntitiesMgr.Instance.GetGameObjectByImageId(31).transform.DOLocalMove(new Vector3(
                        1 * SpawnMgr.gap,
                        0,
                        0 * SpawnMgr.gap
                    ), 0.5f);
                }),
                //TODO:
                DelayAction.Allocate(delayTime: 4f,
                    onDelayFinish: delegate { DialogueMgr.Instance.Log(txt: "山洞？！", 0, 120); }),
                DelayAction.Allocate(delayTime: 4.5f,
                    onDelayFinish: delegate { DialogueMgr.Instance.Log(txt: "你是谁？", 0, 120); }),


                //outro
                /*
                 * ！！单击看全文！！
“我叫五柳。小东西，你家的大人呢？”
“我......我没有爸爸，妈妈被一个叫二郎神的坏人压在那座大山下面了。”
“二郎神？！”
“五柳终至桃花源，却闻沉香失母，又听其详，不禁自问——”
“幼子失母，神降天灾，这算什么桃花源？”
“孩子，都过去那么久了，你一个人是怎么在这荒山野岭里生活的？”
“什么？不是才过了几个时辰吗？”
“五柳再惊，方觉两岸日月有异，正所谓地上一年天上一日。”
”听着，孩子，我有办法了。对岸有颗奇怪的种子，如果两岸时间流转真的不同，那么那种子恐怕已经是通天巨树了！“
（如果房子在遮蔽点）”可那树在哪呢？“
（不在遮蔽点/遮蔽物被移开）“快过去吧！”
                 */
#if ENABLE_ANIM
                DelayAction.Allocate(0.5f, delegate { DialogueMgr.Instance.Log("我叫五柳。小东西，你家的大人呢？", 0, 120); }),
                DelayAction.Allocate(0.5f, delegate { DialogueMgr.Instance.Log("我叫五柳。小东西，你家的大人呢？", 0, 120); }),
                DelayAction.Allocate(0.5f, delegate { DialogueMgr.Instance.Log("我......我没有爸爸，妈妈被一个叫二郎神的坏人压在那座大山下面了。", 0, 120); }),
                DelayAction.Allocate(0.5f, delegate { DialogueMgr.Instance.Log("二郎神？！", 0, 120); }),
                DelayAction.Allocate(0.5f, delegate { DialogueMgr.Instance.Log("五柳终至桃花源，却闻沉香失母，又听其详，不禁自问——", 0, 120); }),
                DelayAction.Allocate(0.5f, delegate { DialogueMgr.Instance.Log("幼子失母，神降天灾，这算什么桃花源？", 0, 120); }),
                DelayAction.Allocate(0.5f, delegate { DialogueMgr.Instance.Log("孩子，都过去那么久了，你一个人是怎么在这荒山野岭里生活的？", 0, 120); }),
                DelayAction.Allocate(0.5f, delegate { DialogueMgr.Instance.Log("什么？不是才过了几个时辰吗？", 0, 120); }),
                DelayAction.Allocate(0.5f, delegate { DialogueMgr.Instance.Log("五柳再惊，方觉两岸日月有异，正所谓地上一年天上一日。", 0, 120); }),
                DelayAction.Allocate(0.5f, delegate { DialogueMgr.Instance.Log("听着，孩子，我有办法了。对岸有颗奇怪的种子，如果两岸时间流转真的不同，那么那种子恐怕已经是通天巨树了！", 0, 120); }),
                DelayAction.Allocate(0.5f, delegate
                {
                    DialogueMgr.Instance.Log("可那树在哪呢？", 0, 120);
                }),
#endif
            })
            .AddCompleteCondition4self(() =>
            {
                // "1.瀑布和山不在同一行（瀑布不在9或14或19）
                //2.愚公和锄头在同一行，并回到黄2d面
                //    （比如，如果愚公在3，则瀑布在12或17即可）"
                if (EntitiesMgr.Instance.GetGameObjectByImageId(28).GetComponent<Entity>().positionY
                    != EntitiesMgr.Instance.GetGameObjectByImageId(26).GetComponent<Entity>().positionY
                    && EntitiesMgr.Instance.GetGameObjectByImageId(28).GetComponent<Entity>().positionY
                    == EntitiesMgr.Instance.GetGameObjectByImageId(31).GetComponent<Entity>().positionY
                    && CameraController.RotateCount == 1
                    && CameraController.viewing_state == ViewingState.V2d)
                {
                    return true; //待写
                }

                return false;
            });
        newLevel.parts.Add(tmpPart);


        //part 2-4
        tmpPart = new Part(new DelayAction[]
            {
                //1.房子image ID
                DelayAction.Allocate(0.5f, () => { DialogueMgr.Instance.Log("快过去吧！", 0, 120); }),

                //沉香 -> 4
                DelayAction.Allocate(0.5f, () =>
                {
                    EntitiesMgr.Instance.GetGameObjectByImageId(21).transform.DOMove(new Vector3(
                        SpawnMgr.gap * -2,
                        0,
                        SpawnMgr.gap * 0
                    ), 0.5f);
                }),
                // 沉香 -> 垂直向上移出
                DelayAction.Allocate(0.5f, delegate
                {
                    EntitiesMgr.Instance.GetGameObjectByImageId(21).transform.DOMove(new Vector3(
                        SpawnMgr.gap * -2,
                        50,
                        SpawnMgr.gap * 2
                    ), 0.5f);
                }),


                //load scene
                DelayAction.Allocate(2f, () => { EntitiesMgr.Instance.ClearAll(); }),
            })
            .AddCompleteCondition4self(() =>
            {
                //房子 ID
                //房子和种子不在同一行
                //if (EntitiesMgr.Instance.GetGameObjectByImageId(52).GetComponent<Entity>().positionY
                //   != EntitiesMgr.Instance.GetGameObjectByImageId(29).GetComponent<Entity>().positionY)

                return true;

            });
        newLevel.parts.Add(tmpPart);

        //part 2-5-init
        tmpPart = new Part(new DelayAction[]
            {
                DelayAction.Allocate(0.5f, () =>
                {
                    //init clouds
                    GameObject cloud1 = Resources.Load<GameObject>("Level1/_cloudA").Instantiate();
                    cloud1.transform.SetParent(EntitiesMgr.Instance.transform);
                    cloud1.transform.localPosition = new Vector3(-1 * SpawnMgr.gap, 0, 2 * SpawnMgr.gap);

                    GameObject cloud2 = Resources.Load<GameObject>("Level1/_cloudA").Instantiate();
                    cloud2.transform.SetParent(EntitiesMgr.Instance.transform);
                    cloud2.transform.localPosition = new Vector3(-2 * SpawnMgr.gap, 0, 1 * SpawnMgr.gap);

                    GameObject cloud3 = Resources.Load<GameObject>("Level1/_cloudB").Instantiate();
                    cloud3.transform.SetParent(EntitiesMgr.Instance.transform);
                    cloud3.transform.localPosition = new Vector3(0 * SpawnMgr.gap, 0, 1 * SpawnMgr.gap);

                    GameObject cloud4 = Resources.Load<GameObject>("Level1/_cloudB").Instantiate();
                    cloud4.transform.SetParent(EntitiesMgr.Instance.transform);
                    cloud4.transform.localPosition = new Vector3(1 * SpawnMgr.gap, 0, 1 * SpawnMgr.gap);

                    GameObject cloud5 = Resources.Load<GameObject>("Level1/_cloudB").Instantiate();
                    cloud5.transform.SetParent(EntitiesMgr.Instance.transform);
                    cloud5.transform.localPosition = new Vector3(0 * SpawnMgr.gap, 0, -1 * SpawnMgr.gap);

                    GameObject cloud6 = Resources.Load<GameObject>("Level1/_cloudB").Instantiate();
                    cloud6.transform.SetParent(EntitiesMgr.Instance.transform);
                    cloud6.transform.localPosition = new Vector3(-1 * SpawnMgr.gap, 0, -1 * SpawnMgr.gap);

                    cloud1.GetComponent<Entity>().imageId = 101;
                    cloud1.GetComponent<Entity>().imageId = 102;
                    cloud1.GetComponent<Entity>().imageId = 103;
                    cloud1.GetComponent<Entity>().imageId = 104;
                    cloud1.GetComponent<Entity>().imageId = 105;
                    cloud1.GetComponent<Entity>().imageId = 106;


                    //init items
                    GameObject erLang = Resources.Load<GameObject>("Level1/_erLang").Instantiate();
                    erLang.transform.SetParent(EntitiesMgr.Instance.transform);
                    erLang.transform.localPosition = new Vector3(-1 * SpawnMgr.gap, 0, 1 * SpawnMgr.gap);

                    GameObject chenXiang = Resources.Load<GameObject>("Level1/_ChenXiang").Instantiate();
                    chenXiang.transform.SetParent(EntitiesMgr.Instance.transform);
                    chenXiang.transform.localPosition = new Vector3(0 * SpawnMgr.gap, 0, 2 * SpawnMgr.gap);
                }),

                DelayAction.Allocate(0.5f, () => { DialogueMgr.Instance.Log("这就是天庭？可是二郎神在哪呢？", 0, 120); }),
                DelayAction.Allocate(0.5f, () => { DialogueMgr.Instance.Log("云？难道他在云的后面吗？", 0, 120); }),
            })
            .AddCompleteCondition4self(() => { return true; });
        newLevel.parts.Add(tmpPart);

        //part 2-5
        tmpPart = new Part(new DelayAction[]
            {
                /*
“这就是天庭？可是二郎神在哪呢？”
“云？难道他在云的后面吗？”
《解开后》
“二郎神？快把妈妈还给我！”
“......不要。”
“你说什么？”
“凡人都是蠢货。”
*/

#if ENABLE_ANIM
                DelayAction.Allocate(0.5f, () => { DialogueMgr.Instance.Log("二郎神？快把妈妈还给我！", 0, 120); }),
                DelayAction.Allocate(0.5f, () => { DialogueMgr.Instance.Log("......不要。", 0, 120); }),
                DelayAction.Allocate(0.5f, () => { DialogueMgr.Instance.Log("你说什么？", 0, 120); }),
                DelayAction.Allocate(0.5f, () => { DialogueMgr.Instance.Log("凡人都是蠢货。", 0, 120); }),
                
                //TODO: 沉香妈妈出现
                DelayAction.Allocate(0.5f, () => { ; }),

                DelayAction.Allocate(0.5f, () => { DialogueMgr.Instance.Log("阿戬，别为难沉香了。", 0, 120); }),
                DelayAction.Allocate(0.5f, () => { DialogueMgr.Instance.Log("妈妈？", 0, 120); }),
                DelayAction.Allocate(0.5f, () => { DialogueMgr.Instance.Log("欲知后事如何，请听下回分解", 0, 120); }),
#endif
            })
            .AddCompleteCondition4self(() =>
            {
                if (EntitiesMgr.Instance.CheckCoordByImageId(101, -1, 2)
                    && EntitiesMgr.Instance.CheckCoordByImageId(102, 0, -2)
                    && EntitiesMgr.Instance.CheckCoordByImageId(103, 1, 1)
                    && EntitiesMgr.Instance.CheckCoordByImageId(104, 2, 1)
                    && EntitiesMgr.Instance.CheckCoordByImageId(105, -1, -1)
                    && EntitiesMgr.Instance.CheckCoordByImageId(106, -1, -2)
                )
                {
                    return true;
                }

                return false;
            });
        newLevel.parts.Add(tmpPart);


        levels.Add(newLevel);
    }


    //**********************************************************************//
    //-----------------------------  Level 3  
    //**********************************************************************//
    private void InitLevel3()
    {
        Level newLevel = new Level();
        //Part tmpPart = new Part();

        //TODO: init temporary parts
        //newLevel.parts.Add(tmpPart);


        levels.Add(newLevel);
    }

    private void InitLevels() //此处应该处理UI逻辑？
    {
        InitLevel1();

        InitLevel2();

        //InitLevel3();
    }

    #endregion


    public GlobalStateMachine()
    {
        InitLevels();
    }
}

public class Part
{
    public bool Ended;
    public bool Invoked;

    public Func<bool> completeCondition;
    private SequenceNode completedSequence;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="sequenceNode"></param>
    public Part(params DelayAction[] delayActions)
    {
        //init
        completedSequence = new SequenceNode();

        //lock input action
        completedSequence.Append(DelayAction.Allocate(0f, () => Main.InputActionAlive = false));
        for (int i = 0; i < delayActions.Length; i++)
        {
            completedSequence.Append(delayActions[i]);
        }

        //release input action
        completedSequence.OnEndedCallback += delegate
        {
            Main.InputActionAlive = true;
            Ended = true;
        };
        //completedSequence.OnEndedCallback += () => { Debug.Log("input action alive"); };
        completeCondition = null;
        Ended = Invoked = false;
    }

    public void AddCompleteCondition(Func<bool> condition)
    {
        completeCondition = condition;
    }

    //invoke Sequence when completed
    public void Complete()
    {
        if (Setup.Instance == null)
        {
            Debug.LogError("null");
            return;
        }

        Invoked = true;
        Setup.Instance.ExecuteNode(completedSequence);
    }
}


public static class PartExtern
{
    public static Part AddCompleteCondition4self(this Part self, Func<bool> condition)
    {
        self.AddCompleteCondition(condition);
        return self;
    }
}


public class Level
{
    public List<Part> parts = new List<Part>();


    /// <summary>
    /// return false if this level completed
    /// </summary>
    /// <param name="curPart"></param>
    /// <returns></returns>
    public bool JumpToNextPart(ref int curPart)
    {
        if (curPart >= parts.Count - 1)
        {
            return false;
        }


        if (!parts[curPart].Ended)
        {
            if (!parts[curPart].Invoked)
            {
                parts[curPart].Complete();
            }

            return true;
        }

        Debug.Log("<color=red>Complete current part :: </color>" + curPart);
        ++curPart;

        return true;
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class BattleSceneManager : MonoBehaviour
{
    public Transform[] spawnPoints; // SpawnPoint 배열
    public PlayableDirector timeline; // Timeline 컨트롤러
    public TimelineAsset timelineAsset; // Timeline Asset

    private void Start()
    {
        // Timeline Asset을 초기화
        if (timelineAsset == null)
        {
            timelineAsset = (TimelineAsset)timeline.playableAsset;
        }

        // 선택된 캐릭터를 Timeline에 바인딩
        for (int i = 0; i < CharacterSelection.selectedCharacters.Length; i++)
        {
            if (CharacterSelection.selectedCharacters[i] != null)
            {
                GameObject character = CharacterSelection.selectedCharacters[i];
                character.SetActive(true);

                // SpawnPoint를 기준으로 초기 위치
                character.transform.position = spawnPoints[i].position;

                // 캐릭터 회전값 설정
                Vector3 rotationOffset = Vector3.zero;

                if (i == 0) rotationOffset = new Vector3(0, 198, 0); // 첫 번째 캐릭터 회전값
                else if (i == 1) rotationOffset = new Vector3(0, 155, 0); // 두 번째 캐릭터 회전값
                else rotationOffset = new Vector3(0, 180, 0); // 기본 회전값

                character.transform.rotation = Quaternion.Euler(rotationOffset);

                // Timeline에 Track 추가 및 바인딩
                AddAnimationTrackToTimeline(character, $"CharacterTrack_{i}", spawnPoints[i].position, rotationOffset);
            }
        }

        // Timeline 재생 
        if (timeline.state != PlayState.Playing)
        {
            Debug.Log("Starting Timeline...");
            timeline.Play(); // Timeline 재생
        }
    }

    private void AddAnimationTrackToTimeline(GameObject character, string trackName, Vector3 initialPosition, Vector3 initialRotation)
    {
        // Timeline에 Animation Track 추가
        var animationTrack = timelineAsset.CreateTrack<AnimationTrack>(null, trackName);

        // 선택된 캐릭터를 Animation Track에 바인딩
        timeline.SetGenericBinding(animationTrack, character);

        // "(Clone)" 제거
        string characterName = character.name.Replace("(Clone)", "").Trim();

        // 애니메이션 경로 리스트
        string[] animationClips = { "Walk", "Select" };
        float currentStartTime = 0; // 클립 시작 시간

        Dictionary<int, Vector3> characterPositions = new Dictionary<int, Vector3>();
        characterPositions[0] = initialPosition; // 첫 번째 캐릭터의 초기 위치
        characterPositions[1] = initialPosition; // 두 번째 캐릭터의 초기 위치

        foreach (string animationClipName in animationClips)
        {
            // 애니메이션 경로 설정
            var animationPath = $"Animations/{characterName}/{characterName}{animationClipName}";
            var animClip = Resources.Load<AnimationClip>(animationPath);

            if (animClip != null)
            {
                var playableAsset = ScriptableObject.CreateInstance<AnimationPlayableAsset>();
                playableAsset.clip = animClip;

                var timelineClip = animationTrack.CreateDefaultClip();
                timelineClip.asset = playableAsset;

                timelineClip.start = currentStartTime; // 시작 시간 설정
                if (animationClipName == "Walk")
                {
                    timelineClip.duration = animClip.length * 8;  
                }
                else if (animationClipName == "Select")
                {
                    timelineClip.duration = animClip.length * 2;  
                }

                AnimationPlayableAsset animAsset = (AnimationPlayableAsset)timelineClip.asset;

                Vector3 currentPosition = characterPositions[Array.IndexOf(CharacterSelection.selectedCharacters, character)];

                if (animationClipName == "Select")
                {
                    int characterIndex = Array.IndexOf(CharacterSelection.selectedCharacters, character);

                    // 캐릭터별 Select 위치 설정
                    if (characterIndex == 0)
                    {
                        if (characterName == "Bodybuilder")
                            currentPosition = new Vector3(-4.91018629f, 0.614367485f, -1.04243553f);
                        else if (characterName == "Rosales")
                            currentPosition = new Vector3(-4.88019609f, 0.567475796f, -0.869782567f);
                        else if (characterName == "Mutant")
                            currentPosition = new Vector3(-6.76947021f, 0.576784492f, 0.523667455f);
                    }
                    else if (characterIndex == 1)
                    {
                        if (characterName == "Bodybuilder")
                            currentPosition = new Vector3(6.53208923f, 0.614367485f, -0.533147156f);
                        else if (characterName == "Rosales")
                            currentPosition = new Vector3(6.70816708f, 0.560555577f, -0.262196302f);
                        else if (characterName == "Mutant")
                            currentPosition = new Vector3(4.10421276f, 0.576784492f, -0.655801833f);
                    }
                }

                if (animationClipName == "Walk")
                {
                    if (characterName == "Bodybuilder")
                    {
                        timelineClip.timeScale = 1.35f;
                    }
                    else if (characterName == "Rosales")
                    {
                        timelineClip.timeScale = 1.05f;
                    }
                    else if (characterName == "Mutant")
                    {
                        timelineClip.timeScale = 0.9f;
                    }
                }
                animAsset.position = currentPosition;
                animAsset.rotation = Quaternion.Euler(initialRotation);
                characterPositions[Array.IndexOf(CharacterSelection.selectedCharacters, character)] = currentPosition;
                // 다음 클립의 시작 시간 업데이트
                currentStartTime += (float)timelineClip.duration;
            }
        }
    }
}

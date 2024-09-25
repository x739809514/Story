using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class AnimPlayable
{
    private PlayableGraph graph;
    private AnimationClip clip;
    private AnimationClipPlayable clipPlayable;

    public AnimPlayable(AnimationClip clip, Animator animator)
    {
        graph = PlayableGraph.Create();
        var output = AnimationPlayableOutput.Create(graph, "output", animator);
        clipPlayable = AnimationClipPlayable.Create(graph, clip);
        output.SetSourcePlayable(clipPlayable);
    }

    public void PlayAnim()
    {
        clipPlayable.SetTime(0);
        graph.Play();
    }

    public void StopAnim()
    {
        graph.Stop();
    }
}
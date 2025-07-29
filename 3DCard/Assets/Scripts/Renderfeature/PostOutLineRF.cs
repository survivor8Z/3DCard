using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostOutLineRF : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        private RTHandle tempRT1; // ��һ����ʱ��Ⱦ����
        public RTHandle _cameraColor;
        private static readonly List<ShaderTagId> s_ShaderTagIds = new List<ShaderTagId>
        {
            new ShaderTagId("SRPDefaultUnlit"),
            new ShaderTagId("UniversalForward"),
            new ShaderTagId("UniversalForwardOnly"),
        };
        public Material m_EdgeDetect;
        private readonly FilteringSettings m_FilteringSettings;
        private readonly MaterialPropertyBlock m_Block;//��������ʴ��ݲ���
        private RTHandle rt;
        public CustomRenderPass()
        {
            renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
            m_FilteringSettings = new FilteringSettings(RenderQueueRange.all);
            m_Block = new MaterialPropertyBlock();
        }
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            RenderTextureDescriptor desc = cameraTextureDescriptor;
            desc.colorFormat = RenderTextureFormat.ARGB32;
            desc.depthBufferBits = 0;
            RenderingUtils.ReAllocateIfNeeded(ref tempRT1, desc, name: "TempTexture1");
        }
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ResetTarget();
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.msaaSamples = 1;//;����Ҫ̫�࿹���
            desc.depthBufferBits = 0;//���������
            desc.colorFormat = RenderTextureFormat.ARGB32;//��֤��alphaͨ��
            RenderingUtils.ReAllocateIfNeeded(ref rt, desc, name: "DetectRT");//������Ϣ���·���RT(���´���һ������˼)
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("Detect Command");//����һ����Ⱦ����(cmd����д����)

            var cameraColorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;

            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            //��Ե���
            m_EdgeDetect.SetTexture("_MainTex", cameraColorTarget);
            Blitter.BlitCameraTexture(cmd, cameraColorTarget, tempRT1);
            Blitter.BlitCameraTexture(cmd, tempRT1, cameraColorTarget, m_EdgeDetect, 0);

            context.ExecuteCommandBuffer(cmd);//ִ��

            cmd.Clear(); ;
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
        public void Setup(RTHandle cameraColor)
        {
            _cameraColor = cameraColor;
        }

        public void Dispose()
        {
            if (rt != null)
            {
                rt.Release();
                rt = null;
            }
        }
    }

    CustomRenderPass m_Pass;
    [SerializeField] Material m_Edge;

    private bool isValid => m_Edge && m_Edge.shader && m_Edge.shader.isSupported;
    public override void Create()
    {
        if (!isValid) return;
        m_Pass = new CustomRenderPass();
        m_Pass.m_EdgeDetect = m_Edge;
    }

    //�����Ͽ���˵��ÿ֡����
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (m_Pass == null) return;
        if (!ShouldExecute(renderingData)) return;
        renderer.EnqueuePass(m_Pass);
    }
    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        if (m_Pass == null) return;
        if (!ShouldExecute(renderingData)) return;
        m_Pass.Setup(renderer.cameraColorTargetHandle);

    }
    protected override void Dispose(bool disposing)
    {
        m_Pass?.Dispose();
    }
    bool ShouldExecute(in RenderingData data)
    {
        if (data.cameraData.cameraType != CameraType.Game)
        {
            return false;
        }
        return true;
    }
}



using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using YooAsset;

public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance;
	private cfg.TbAudio audioConfigMap => GameManager.Tables.TbAudio;
	private AudioSource musicSource; // 播放音乐
	private List<AudioSource> sfxSources; // 音效播放源池

	[Header("Audio Source Pool Settings")]
	public int initialSfxSourceCount = 5; // 初始音效播放源池大小

	public List<cfg.Audio> bgmCfgList = new List<cfg.Audio>();

	private void Awake()
	{
		// 确保单例
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
			return;
		}

		// 初始化音频源
		musicSource = gameObject.AddComponent<AudioSource>();
		// 初始化音效音频源池
		sfxSources = new List<AudioSource>();
		for (int i = 0; i < initialSfxSourceCount; i++)
		{
			AudioSource source = gameObject.AddComponent<AudioSource>();
			sfxSources.Add(source);
		}

		bgmCfgList = audioConfigMap.DataList.FindAll((cfg) => { return cfg.IsGameBGM; });
	}

	protected void Start()
	{
		Mute(true, !GameManager.gameData.enableMusic);
		Mute(false, !GameManager.gameData.enableSound);
	}

	// 播放音效的接口
	public void Play(int audioId)
	{
		cfg.Audio audio = audioConfigMap.GetOrDefault(audioId);
		if (audio == null)
		{
			LTDebug.LogError($"音效ID{audioId}资源不存在");
			return;
		}
		LoadAndPlayAudio(audio);
	}

	private void LoadAndPlayAudio(cfg.Audio config)
	{
		var package = YooAssets.GetPackage("DefaultPackage");
		var handle = package.LoadAssetSync<AudioClip>(config.Path);

		if (handle.AssetObject == null)
		{
			LTDebug.LogError($"音效{config.Id} path {config.Path} 不对");
			return;
		}

		AudioClip clip = handle.AssetObject as AudioClip;

		if (config.IsMusic)
			PlayMusic(clip, config.Volume);
		else
			PlaySfx(clip, config.Volume);
	}

	// 播放音乐（独占一个音乐通道）
	private void PlayMusic(AudioClip clip, float volume)
	{
		musicSource.clip = clip;
		musicSource.volume = volume;
		musicSource.loop = true;
		musicSource.Play();
	}

	// 播放音效（从音效池中选择一个空闲的音频源）
	private void PlaySfx(AudioClip clip, float volume)
	{
		AudioSource source = GetAvailableSfxSource();
		if (source != null)
		{
			source.clip = clip;
			source.volume = volume;
			source.loop = false; // 音效通常不需要循环
			source.Play();

			// 自动释放音效播放源
			StartCoroutine(ReleaseSfxSourceWhenDone(source));
		}
		else
		{
			Debug.LogWarning("音效播放源已用尽！");
		}
	}

	// 自动释放音效播放源
	private IEnumerator ReleaseSfxSourceWhenDone(AudioSource source)
	{
		while (source.isPlaying)
		{
			yield return null;
		}
		source.clip = null; // 清理资源
	}

	// 获取一个可用的音效播放源
	private AudioSource GetAvailableSfxSource()
	{
		foreach (var source in sfxSources)
		{
			if (!source.isPlaying)
				return source;
		}

		// 如果没有可用的音效播放源，则动态添加新的音效播放源
		AudioSource newSource = gameObject.AddComponent<AudioSource>();
		sfxSources.Add(newSource);
		return newSource;
	}

	/// <summary>
	/// 禁音
	/// </summary>
	/// <param name="isMusic">是否音乐</param>
	/// <param name="isMute">是否静音</param>
	public void Mute(bool isMusic, bool isMute)
	{
		if (isMusic)
		{
			musicSource.mute = isMute;
		}
		else
		{
			foreach (var sfx in sfxSources)
			{
				sfx.mute = isMute;
			}
		}
	}
}


# SecretLabNAudio.YouTube Demo

This project showcases a fairly simple use case of the SecretLabNAudio.YouTube module.

The `PlaybackManager` manages video (audio) playback; everyone can hear the audio.

Players with the `yt.play` permission can play a video, pause playback, or stop it.
The command is available through the Remote Admin and the client console as well.

- `yt play <video ID or URL>` - play a video
- `yt play` - toggle playback (pause/resume)
- `yt stop` - stop playback
- `yt search <query>` - search for videos
- `yt myResults` - list the results in your previous query

Search lists the top 10 results. Use `yt play <index>` to play a video you searched for
(e.g. `yt play 2` plays the 2nd result).

When a video is played for the first time, it will be cached, allowing for instantaneous playback
the next time the video is requested.

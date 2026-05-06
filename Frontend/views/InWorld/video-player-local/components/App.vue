<script setup lang="ts">
import TuckInVideo from '@shared/assets/video/tuck_in.webm'
import {onMounted, ref, onBeforeUnmount} from "vue";

const currentVideo = ref(TuckInVideo)
const timeout = ref(0)

const unityVideo = 'coui://UIResources/VideoSources/video-example.webm'
// const unityVideo = 'coui://UIResources/VideoSources/tuck_in.webm'

onMounted(() => {
  // @ts-expect-error Timeout is not number
  timeout.value = setInterval(() => {
    if (!unityVideo) {
      console.warn('No unityVideo in Unity Editor')
      currentVideo.value = TuckInVideo
      return
    }

    if (currentVideo.value === TuckInVideo) {
      currentVideo.value = unityVideo
      return
    }

    currentVideo.value = TuckInVideo
  }, 3000)
})

onBeforeUnmount(() => {
  clearInterval(timeout.value)
})
</script>

<template>
  <div class="video_player">
    <video :src="currentVideo" width="200" height="400" autoplay loop muted playsinline />
  </div>
</template>

<style lang="scss">
@use 'shared/styles/_reset';
</style>

<style lang="scss" scoped>
.video_player {
  width: 100%;
  aspect-ratio: 2/1;
}
</style>

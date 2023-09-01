#!/usr/bin/env zx

const fromDirectory = "android-dependency-generator/Assets/Plugins/Android";
const toDirectory = "haechi.face.unity.sdk/Plugins/Android"
await $`ls ${toDirectory}`
await $`cp -f ${fromDirectory}/* ${toDirectory}/`
await $`ls ${toDirectory}`


<template>
    <el-dialog
            title="图片预览"
            :width="isMobile ? '100%' : '900px'"
            center
            :visible="visible"
            :append-to-body="appendToBody"
            :before-close="cancel"
    >
        <div class="preview">
            <el-carousel :interval="0" :height="isMobile ? '300px' : '600px'" :autoplay="false" arrow="always">
                <el-carousel-item v-for="item in images" :key="item">
                    <a :href="item" target="_blank" class="image" >
                        <img :src="item"/>
                    </a>
                </el-carousel-item>
            </el-carousel>
        </div>
    </el-dialog>
</template>
<style lang="scss" scoped>
    .image {
        display: block;
        height: 100%;
        background-position: center;
        background-repeat: no-repeat;
        background-size: contain;
        img{
            display: block;
            object-fit: contain;
            height: 100%;
            max-width: 100%;
            margin: auto;
        }
    }
</style>
<script>
    export default {
        props: {
            appendToBody: {
                default: false
            },
            isMobile: {
                default: false
            },
            images: {
                default() {
                    return []
                }
            }
        },
        data() {
            return {
                visible: true
            }
        },
        methods: {
            close() {
                this.visible = false;
            },
            cancel() {
                this.close();
                this.$emit('cancel', false);
            }
        }
    }
</script>
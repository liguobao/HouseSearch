<template>
    <el-dialog
            title="地图搜租房"
            width="100%"
            center
            :fullscreen="true"
            :visible="visible"
            :append-to-body="appendToBody"
            :before-close="cancel"
            class="house-list"
    >
        <div class="">
            <ul class="list">
                <li v-for="item in list" :key="`${item.id}-${item.source}`">
                    <div class="left">
                        <template v-if="item.pictures && item.pictures.length">
                            <transition name="el-fade-in">
                                <i v-show="!imagesLoadingMap[item.pictures[0]]" class="el-icon-loading loading-icon"></i>
                            </transition>
                            <transition name="el-fade-in">
                                <img :src="item.pictures[0]"
                                     v-show="imagesLoadingMap[item.pictures[0]]"
                                     @load="imageLoading(item.pictures[0],imagesLoadingMap)"/>
                            </transition>
                        </template>

                    </div>
                    <div class="right">
                        <div class="content">
                            <a class="title" :href="item.houseOnlineURL" target="_blank">{{item.houseTitle ?
                                item.houseTitle : item.houseLocation}}</a>
                            <div class="price" v-if="item.disPlayPrice">
                                {{item.disPlayPrice}}<span> /月</span>
                            </div>
                        </div>
                        <div class="source">
                            来源: {{item.displaySource}}
                        </div>
                    </div>
                </li>
            </ul>
        </div>
    </el-dialog>
</template>
<style lang="scss" scoped>
    .house-list {

        /deep/ .el-dialog__body {
            padding: 0;
            background: rgb(248, 248, 248);
        }
        .list {
            max-height: 89vh;
            overflow: auto;
            -webkit-overflow-scrolling: touch;
            padding: 10px;
            li {
                display: flex;
                background: #fff;
                margin-bottom: 10px;
                border-radius: 4px;
                padding: 6px;
            }
        }
        .left {
            width: 100px;
            flex: none;
            img {
                display: block;
                max-width: 100%;
                border-radius: 4px;
            }
        }
        .right {
            margin-left: 10px;
            flex: 1;
            width: 100%;
            overflow: hidden;
            position: relative;
            .title {
                color: #000000;
                display: block;
                overflow: hidden;
                white-space: nowrap;
                max-width: 100%;
                text-overflow: ellipsis;
            }
            .price {
                color: #E31818;
                font-size: 16px;
                span {
                    font-size: 12px;
                }
            }
            .source{
                font-size: 10px;
                color: #AAAAAA;
                position: absolute;
                right: 6px;
                bottom: 0px;
                text-align: right;
            }
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
            list: {
                default() {
                    return []
                }
            }
        },
        data() {
            return {
                imagesLoadingMap: {},
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
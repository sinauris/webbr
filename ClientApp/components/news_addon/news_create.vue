<template>
    <div>
        <h2>Создание новости</h2>
        <hr/>
        <froala :tag="'textarea'" :config="titleConfig" v-model="titleModel"></froala>
        <br/>
        <froala :tag="'textarea'" :config="bodyConfig" v-model="bodyModel"></froala>

        <div class="mt-4 ">
            <div class="float-left">
                <span class="text-danger">{{ createErrorMessage }}</span>
            </div>

            <div class="float-right">
                <b-btn v-if="!createLoadingState" size="sm" variant="primary" @click="saveCreatedNews">
                    <font-awesome-icon class="mr-1" icon="save" /> Создать новость
                </b-btn>
                <b-btn v-else size="sm" variant="primary" @click="saveEditedNews" style="min-width: 162px" disabled>
                    <font-awesome-icon icon="cog" spin/>
                </b-btn>
            </div>
        </div>
    </div>
</template>

<script>
    import { USER_POST } from '../../store/actions/user'

    export default {
        data() {
            return {
                titleConfig: {
                    pluginsEnabled: ['charCounter'],
                    toolbarInline: true,
                    language: 'ru',
                    multiLine: false,
                    placeholderText: 'Заголовок',
                    charCounterCount: true,
                    charCounterMax: 140,
                    quickInsertTags: [],
                    toolbarButtons: [],
                },
                bodyConfig: {
                    pluginsEnabled: ['charCounter', 'colors', 'image', 'link', 'file', 'table', 'lists', 'align', 'quote', 'paragraphFormat', 'help'],
                    language: 'ru',
                    heightMin: 400,
                    heightMax: 550,
                    charCounterCount: true,
                    charCounterMax: 10000,
                    quickInsertTags: [],
                    tabSpaces: 4,
                    linkEditButtons: ['linkOpen', 'linkEdit', 'linkRemove'],
                    linkInsertButtons: ['linkBack'],
                    listAdvancedTypes: false,
                    tableEditButtons: ['tableHeader', 'tableRows', 'tableColumns', 'tableCells', 'tableCellVerticalAlign', 'tableCellHorizontalAlign', 'tableRemove'],
                    tableInsertButtons: [],
                    toolbarButtons: ['paragraphFormat', '|', 'bold', 'italic', 'underline', 'strikeThrough', 'subscript', 'superscript', 'color', '|', 'align', 'formatOL', 'formatUL', 'outdent', 'indent', 'quote', 'insertLink', 'insertImage', 'insertFile', 'insertTable', 'insertHR', 'clearFormatting', '|', 'undo', 'redo'],

                    imageUploadURL: '/api/index/uploadimage',
                    imageMaxSize: 1024 * 1024 * 10,
                    imageAllowedTypes: ['jpeg', 'jpg', 'png', 'gif'],
                    imageEditButtons: ['imageReplace', 'imageAlign', 'imageDisplay', 'imageCaption', 'imageRemove'],

                    fileUploadURL: '/api/index/uploadfile',
                    fileAllowedTypes: ['*'],
                    fileMaxSize: 100 * 1024 * 1024,

                    requestHeaders: { Authorization: `Bearer ${localStorage.getItem('webbr-token')}` }
                },

                titleModel: '',
                bodyModel: '',
                createLoadingState: false,
                createErrorMessage: ''
            }
        },
        methods: {
            async saveCreatedNews() {
                this.createLoadingState = true;

                let id = this.$route.params.id;
                let title = this.titleModel;
                let body = this.bodyModel;

                await this.$store.dispatch(USER_POST, {
                    url: '/api/index/createnews',
                    id: id,
                    title: title,
                    body: body
                })
                    .then(async (resp) => {
                        this.createLoadingState = false;
                        this.$emit('reloadNews');
                        this.$router.push('/news/' + resp.data);
                    })
                    .catch((err) => {
                        this.createLoadingState = false;
                        this.createErrorMessage = err.response.data;
                    });
            },
        },
        computed: {

        },
        mounted() {

        },

        activated() {

        },
        async created() {

        }
    }
</script>

<style>
    .fr-counter {
        display: none !important;
    }
    .fr-inline {
        font-size: 24px;
    }
    .fr-inline > .fr-wrapper > .fr-view {
        padding-left: 16px !important;
    }
    .fr-basic {
        border: 1px solid #ced4da;
    }
    .fr-toolbar {
        border-top: 0 solid #ced4da !important;
        border-bottom: 1px solid #ced4da !important;
    }
    .fr-popup{
        border: solid #222 !important;
        border-width: 5px 2px 2px 2px !important;
    }

    .fr-box.fr-basic.fr-top div:not([class^="fr-"]) {
        display:none
    }
</style>

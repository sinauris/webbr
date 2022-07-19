<template>
    <b-row>
        
        <b-col cols="12" sm="2" md="2" lg="2" xl="2">
            <br/><br/>
            <div style="margin-left: 80px">
                <b-nav vertical>
                    <template v-if="isRgPolicy">
                        <h6 style="cursor: default;">Система</h6>
                        <b-nav-item active-class="activeNav" to="/configuration/users">Пользователи</b-nav-item>
                        <b-nav-item active-class="activeNav" to="/configuration/places">Площадки</b-nav-item>
                    </template>
    
                    <h6 class="mt-3" style="cursor: default;">Коммутаторы</h6>
                    <b-nav-item active-class="activeNav" v-for="p in getPlace" :key="p.placeid" :to="'/configuration/sw/' + p.place_short_description">{{ p.place_description }}</b-nav-item>
    
    <!--                <h6 class="mt-3" style="cursor: default;">Телефония</h6>-->
    <!--                <b-nav-item active-class="activeNav" to="/configuration/telephony/aon">Список АОНов</b-nav-item>-->
                </b-nav>
            </div>
        </b-col>
        
        <b-col cols="11" sm="10" md="10" lg="10" xl="9">
            <keep-alive>
                <router-view/>
            </keep-alive>
        </b-col>
    </b-row>
</template>

<script>
    import { mapGetters } from 'vuex'
    export default {
        data () { return {} },
        computed: {
            ...mapGetters(['isOtpOnlyPolicy', 'isRgPolicy', 'getPlace'])
        }
    }
</script>

<style scoped>
    .activeNav {
        color: black;
        font-weight: bold;
        background-color: #eeeeee;
        border-left: 5px groove #f3f3f3;
    }

    .nav-link {
        padding-bottom: 2px;
        padding-top: 2px;
        font-size: 0.8rem;
    }
</style>
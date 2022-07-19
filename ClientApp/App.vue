<template>
    <div class="mb-5">
        <navbar/>
        
        <keep-alive>
            <router-view />
        </keep-alive>
    </div>
</template>

<script>
    import { mapGetters } from 'vuex'
    import { USER_GET, USER_PLACE_ACTION, USER_PORTMAP_NAV_ACTION, USER_INVENTORY_NAV_ACTION, USER_DUTY_NAV_ACTION, USER_DOMAIN_NAV_ACTION } from './store/actions/user'
    import { SIGNALR_START } from './store/actions/signalr'
    import Navbar from "./components/navbar";

    
    export default {
        data() {
            return {
                
            };
        },
        components: { 
            Navbar 
        },
        computed: {
            ...mapGetters(['isAuthenticated', 'isGuestPolicy', 'getTokenPlace', 'getDomainNav', 'isRgPolicy']),
        },
        async created() {
            if(this.isAuthenticated) {
                await this.$store.dispatch(USER_GET, '/api/configuration/getavailableplaces')
                    .then(async (resp) => {
                        this.$store.dispatch(USER_PLACE_ACTION, resp.data);

                        let pl = resp.data.filter(a => a.place_description === this.getTokenPlace);
                        if (pl.length !== 0) {
                            let shortDescr = pl[0].place_short_description;
                            await this.$store.dispatch(USER_PORTMAP_NAV_ACTION, '/portmaps/' + shortDescr);
                            await this.$store.dispatch(USER_INVENTORY_NAV_ACTION, '/inventory/' + shortDescr);
                            if (this.isRgPolicy) await this.$store.dispatch(USER_DUTY_NAV_ACTION, '/duty/' + shortDescr);
                            else await this.$store.dispatch(USER_DUTY_NAV_ACTION, '/duty');
                        }
                        else {
                            await this.$store.dispatch(USER_PORTMAP_NAV_ACTION, '/portmaps');
                            await this.$store.dispatch(USER_INVENTORY_NAV_ACTION, '/inventory');
                            await this.$store.dispatch(USER_DUTY_NAV_ACTION, '/duty');
                        }
                        if(this.getDomainNav === '') await this.$store.dispatch(USER_DOMAIN_NAV_ACTION, '/domain');

                        await this.$store.dispatch(SIGNALR_START);
                    }
                );
            }
        }
    };
</script>


<style>
    * {
        box-shadow: none !important;
        outline: 0 auto -webkit-focus-ring-color !important;
    }
    .fr-inline > .fr-arrow {
        display: none !important;
    }

    .collapsed > .when-opened, :not(.collapsed) > .when-closed { display: none !important }

    .tooltip { font-size: 12px !important }
    .card-header-progress { padding: 1px 0 1px 0 !important }
    .hidden_header { display: none }
    
    .col,
    .col-1,
    .col-2,
    .col-3,
    .col-4,
    .col-5,
    .col-6,
    .col-7,
    .col-8,
    .col-9,
    .col-10,
    .col-11,
    .col-12,
    .col-auto,
    .col-lg,
    .col-lg-1,
    .col-lg-2,
    .col-lg-3,
    .col-lg-4,
    .col-lg-5,
    .col-lg-6,
    .col-lg-7,
    .col-lg-8,
    .col-lg-9,
    .col-lg-10,
    .col-lg-11,
    .col-lg-12,
    .col-lg-auto,
    .col-md,
    .col-md-1,
    .col-md-2,
    .col-md-3,
    .col-md-4,
    .col-md-5,
    .col-md-6,
    .col-md-7,
    .col-md-8,
    .col-md-9,
    .col-md-10,
    .col-md-11,
    .col-md-12,
    .col-md-auto,
    .col-sm,
    .col-sm-1,
    .col-sm-2,
    .col-sm-3,
    .col-sm-4,
    .col-sm-5,
    .col-sm-6,
    .col-sm-7,
    .col-sm-8,
    .col-sm-9,
    .col-sm-10,
    .col-sm-11,
    .col-sm-12,
    .col-sm-auto,
    .col-xl,
    .col-xl-1,
    .col-xl-2,
    .col-xl-3,
    .col-xl-4,
    .col-xl-5,
    .col-xl-6,
    .col-xl-7,
    .col-xl-8,
    .col-xl-9,
    .col-xl-10,
    .col-xl-11,
    .col-xl-12,
    .col-xl-auto {
        position: relative !important;
        width: 100% !important;
        min-height: 1px !important;
        padding-right: 0 !important;
        padding-left: 0 !important;
    }
    .row {
        margin-left: 0 !important;
        margin-right: 0 !important;
    }

    .pr-2, .px-2 {
        padding-right: 0.5rem !important;
    }
    
    .table {
        /*margin-bottom: 0.7rem !important;*/
    }

    .table th {
        padding: 6px 8px !important;
    }
    
    .table td {
        font-size: 13px !important;
        padding: 1px 8px;
    }
    
    .custom-select-sm {
        -webkit-appearance: none !important;
        -moz-appearance: none !important;
        font-size: 14px !important;
        padding: 0 12px !important;
    }
    
    .very-sm {
        font-size: 11px !important;
        padding: 0.15rem 0.5rem !important;
    }

    .popover-header { text-align: center !important }

    html {
        overflow-x: hidden !important;
        margin-right: calc(-1 * (100vw - 100%)) !important;
        min-height: 100% !important;
    }

    .noselect {
        -webkit-touch-callout: none;
        -webkit-user-select: none;
        -khtml-user-select: none;
        -moz-user-select: none;
        -ms-user-select: none;
        user-select: none;
    }
    
    .tooltip-inner{ max-width: 600px !important }
    .popover { max-width: 600px !important }
    
    .popover-body-overflow { max-height: 20rem !important; overflow-y: auto !important }
    .custom-select[multiple], .custom-select[size]:not([size="1"]) { height: calc(1.8125rem + 2px) }
    .betterButton { padding: 0 !important }
    .defaultCursor { cursor: default !important }

    .btnHover { display: none }
    .btnHidden:hover >>> .btnHover { display: block; position: absolute }
    
    .cardAlarm { animation: cardAlarmAnima 1s step-end infinite alternate }
    @keyframes cardAlarmAnima {
        0% { transition: all 0.1s linear, box-shadow 0.1s linear }
        50% { 
            -webkit-border-radius: 0;
            -moz-border-radius: 0;
            border-radius: 0;
            outline: 12px solid red;
            outline-offset: -4px;
            border-color: red; 
            background-color: red 
        }
    }

    .borderAlarm { animation: borderAlarmAnima 1s step-end infinite alternate }
    @keyframes borderAlarmAnima {
        0% { transition: border 0.1s linear, box-shadow 0.1s linear }
        50% {
            -webkit-border-radius: 0;
            -moz-border-radius: 0;
            border-radius: 0;
            outline: 12px solid red;
            outline-offset: -4px;
            box-shadow: 0 0 20px rgba(0,0,0,0.4) inset !important;
            border-color: red; 
        }
    }
</style>
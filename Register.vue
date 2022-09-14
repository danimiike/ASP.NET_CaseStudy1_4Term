<template>
<img :src="`/store.png`" class="store-image" />
    <div class="container">
        <div class="heading">Register</div>
        <div class="status">{{ state.status }}</div>
        <div class="p-fluid">
            <div class="p-field">
                <label for="firstname" class="p-field-label">First Name</label>
                <InputText
                    placeholder="enter first name"
                    id="firstname"
                    v-model="state.firstname"
                    :class="{ 'p-invalid': state.validationErrors.firstname }"
                />
                <small v-show="state.validationErrors.firstname" class="p-error"
                    >Firstname is required.</small
                >
            </div>
            <div class="p-field">
                <label for="lastname" class="p-field-label">Lastname</label>
                <InputText
                    placeholder="enter last name"
                    id="lastname"
                    v-model="state.lastname"
                    :class="{ 'p-invalid': state.validationErrors.lastname }"
                />
                <small v-show="state.validationErrors.lastname" class="p-error"
                    >Lastname is required.</small
                >
            </div>
            <div class="p-field">
                <label for="lastname" class="p-field-label">Email</label>
                <InputText
                    placeholder="enter email address"
                    id="email"
                    v-model="state.email"
                    :class="{ 'p-invalid': state.validationErrors.email }"
                />
                <small v-show="state.validationErrors.email" class="p-error"
                    >valid format is required.</small
                >
            </div>
            <div class="p-field">
                <label for="password" class="p-field-label">Password</label>
                <span class="p-input-icon-right">
                    <InputText
                        placeholder="enter password"
                        id="password"
                        :type="state.visibility"
                        v-model="state.password"
                        :class="{
                            'p-invalid': state.validationErrors.password,
                        }"
                    />
                    <i
                        v-if="state.visibility === 'password'"
                        class="pi pi-eye"
                        @click="showPassword"
                    />
                    <i
                        v-if="state.visibility === 'text'"
                        class="pi pi-eye-slash"
                        @click="hidePassword"
                    />
                </span>
                <small v-show="state.validationErrors.email" class="p-error"
                    >Password is required.</small
                >
            </div>
            <Button
                label="Register"
                @click="validateForm"
                class="p-button-outlined"
                style="margin: 5vh; width: 25vw"
            />
        </div>
    </div>
</template>
<script>
import { reactive } from "vue";
import { poster } from "../util/apiutil";
export default {
    setup() {
        let state = reactive({
            status: "",
            validationErrors: {},
            firstname: "",
            lastname: "",
            email: "",
            password: "",
            visibility: "password",
        });
        const validateForm = async () => {
            !state.firstname.trim()
                ? (state.validationErrors["firstname"] = true)
                : delete state.validationErrors["firstname"];
            !state.lastname.trim()
                ? (state.validationErrors["lastname"] = true)
                : delete state.validationErrors["lastname"];
            !state.email.trim()
                ? (state.validationErrors["email"] = true)
                : delete state.validationErrors["email"];
            !state.password.trim()
                ? (state.validationErrors["password"] = true)
                : delete state.validationErrors["password"];
            if (Object.keys(state.validationErrors).length === 0) {
                state.status = "registering with server";
                let userHelper = {
                    firstname: state.firstname,
                    lastname: state.lastname,
                    email: state.email,
                    password: state.password,
                };
                try {
                    let payload = await poster("register", userHelper);
                    state.status = payload.token;
                } catch (err) {
                    state.status = err.message;
                }
            }
        };
        const showPassword = () => {
            state.visibility = "text";
        };
        const hidePassword = () => {
            state.visibility = "password";
        };
        return {
            state,
            validateForm,
            showPassword,
            hidePassword,
        };
    },
};
</script>
<template>
	<div class="container">
		<img :src="`/store.png`" class="store-image" />
		<div class="heading">Cart Contents</div>
		<img
			class="item-image"
			style="margin-top: 2vh; width: 5vw"
			src="../assets/cart.png"
		/>
		
		<div class="status">{{ state.status }}</div>

		<div v-if="state.cart.length > 0" id="cart">
			<DataTable
				:value="state.cart"
				:scrollable="true"
				scrollHeight="38vh"
				dataKey="id"
				class="p-datatable-striped"
				id="cart-contents"
			>
				<Column class="" header="Name" field="item.productName" style="min-width:35vw; font-size: 3.4vw"/>
				<Column name="col2" header="Qty" field="qty" style="min-width:15vw; font-size: 3.5vw"/>
				<Column name="col3" header="Price" field="strMSRP" style="min-width:20vw; font-size: 3.5vw;"/>
				<Column name="col4" header="Extended" field="extended" style="min-width:25vw; font-size: 3.5vw;"/>
			</DataTable>
		</div>
		<div v-if="state.cart.length > 0">
			<!-- <div class="cart-head">Cart Totals</div>     -->
			<table>
				<tr>
					<td style="width: 20%; font-weight: bold; font-size: 4.0vw; text-align: right;padding-top: 3vw;padding-bottom: 4vw">Subtotal:</td>
					<td
						style="
							width: 10%;
							text-align: right;
							padding-right: 3vw;
              font-size: 4.0vw;
						"
					>
						{{ formatCurrency(state.subtotal) }}
					</td>
				</tr>
				<tr>
					<td style="width: 20%; font-weight: bold; font-size: 4.0vw; text-align: right;padding-bottom: 4vw">Tax:</td>
					<td
						style="
							width: 10%;
							text-align: right;
							padding-right: 3vw;
              padding-bottom: 4vw;
              font-size: 4.0vw;
						"
					>
						{{ formatCurrency(state.tax) }}
					</td>
				</tr>
				<tr>
					<td style="width: 20%; font-weight: bold; font-size: 4.0vw; text-align: right;padding-bottom: 3vw">Total:</td>
					<td
						style="
							width: 10%;
							text-align: right;
							padding-right: 3vw;
              padding-bottom: 3vw;
              font-size: 4.0vw;
						"
					>
						{{ formatCurrency(state.total) }}
					</td>
				</tr>
			</table>
			<Button
                style="margin-top: 2vh"
                label="Add Order"
                @click="saveOrder"
                class="p-button-outlined margin-button1"
            />
            &nbsp;
			<Button
				style="margin-top: 2vh"
				label="Empty Cart"
				@click="clearCart"
				class="p-button-outlined margin-button1"
			/>
		</div>
	</div>
</template>
<script>
import { reactive, onMounted } from "vue";
import { poster } from "../util/apiutil";

export default {
	setup() {
		let state = reactive({
			status: "",
  		subtotal: 0,
			tax: 0,
			total: 0,
			cart: [],
		});
		onMounted(() => {
			if (sessionStorage.getItem("cart")) {
				state.cart = JSON.parse(sessionStorage.getItem("cart"));
				state.cart.map((cartItem) => {
					state.subtotal += cartItem.item.msrp * cartItem.qty;
				});
				state.tax = state.subtotal * 0.13;
				state.total = state.subtotal + state.tax;
			} else {
				state.cart = [];
			}
		});
		const calcExtended = () => {
			state.extended = `${formatCurrency(
				state.qty * state.selectItem.msrp
			)}`;
		};
		const clearCart = () => {
			sessionStorage.removeItem("cart");
			state.cart = [];
			state.status = "cart cleared";
		};
		 const saveOrder = async () => {
            let customer = JSON.parse(sessionStorage.getItem("customer"));
            let order = JSON.parse(sessionStorage.getItem("cart"));
            try {
                state.status = "sending order info to server";
                let orderHelper = { email: customer.email, selections: order };
                let payload = await poster("order", orderHelper);
                if (payload.indexOf("not") > 0) {
                    state.status = payload;
                } else {
                    clearCart();
                    state.status = payload;
                }
            } catch (err) {
                console.log(err);
                state.status = `Error add order: ${err}`;
            }
        };
		const formatCurrency = (value) => {
			return value.toLocaleString("en-US", {
				style: "currency",
				currency: "USD",
			});
		};
		return {
			state,
			clearCart,
			formatCurrency,
			calcExtended,
			saveOrder,
		};
	},
};
</script>

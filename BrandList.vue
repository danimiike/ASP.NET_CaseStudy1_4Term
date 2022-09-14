<template>
  <div>
    <img :src="`/store.png`" class="store-image" />
    <div class="heading">Our Brands</div>
    <div class="status">{{ state.status }}</div>
    <Dropdown
      v-if="state.brands.length > 0"
      v-model="state.selectedBrand"
      style="text-align: left"
      :options="state.brands"
      optionLabel="name"
      optionValue="id"
      placeholder="Select Brand"
      @change="loadProductitems"
    />
    <div style="margin-top: 2vh" v-if="state.productitems.length > 0">
      <DataTable
        :value="state.productitems"
        :scrollable="true"
        scrollHeight="60vh"
        selectionMode="single"
        class="p-datatable-striped"
        @row-select="onRowSelect"
      >
        <Column style="margin-right: -35vw; border-right:none;">
          <template #body="slotProps">
            <img
              :src="`/img/${slotProps.data.graphicName}`"
              :alt="slotProps.data.productName"
              class="product-image"
            />
          </template>
        </Column>
        <Column field="productName" header="Product Name"></Column>
      </DataTable>
      <Dialog v-model:visible="state.itemSelected" class="dialog-border">
        <div style="text-align: center">
          <div style="margin-bottom: 2vh; font-size: larger; font-weight: bold">
            {{ state.selectedItem.productName }} -
            {{ formatCurrency(state.selectedItem.msrp) }}
          </div>
          <img
            :src="`/img/${state.selectedItem.graphicName}`"
            :alt="state.selectedItem.productName"
            class="product-image2"
          />
        </div>
        <table style="margin-top: 2vh; border-">
          <tr>
            <td style="width: 10%; text-align: left; padding-right: 3vw">
              {{ state.selectedItem.description }}
            </td>
          </tr>
        </table>
        <div style="margin-top: 2vh; text-align: center">Enter Qty:</div>
        <div style="margin-top: 2vh; text-align: center">
          <span>
            <InputNumber
              id="qty"
              :min="0"
              v-model="state.qty"
              :step="1"
              incrementButtonClass="plus"
              showButtons
              buttonLayout="horizontal"
              decrementButtonIcon="pi pi-minus"
              incrementButtonIcon="pi pi-plus"
            />
          </span>
        </div>
        <div style="text-align: center; margin-top: 2vh">
          <Button
            label="Add To Cart"
            @click="addToCart"
            class="p-button-outlined margin-button1"
          />
          &nbsp;
          <Button
            label="View Cart"
            class="p-button-outlined margin-button1"
            v-if="state.cart.length > 0"
            @click="viewCart"
          />
        </div>
        <div
          style="text-align: center"
          v-if="state.dialogStatus !== ''"
          class="dialog-status"
        >
          {{ state.dialogStatus }}
        </div>
      </Dialog>
    </div>
  </div>
</template>
<script>
import { reactive, onMounted } from "vue";
import { fetcher } from "../util/apiutil";
import { useRouter } from "vue-router";
export default {
  setup() {
    onMounted(() => {
      loadBrands();
    });
    let state = reactive({
      status: "",
      brands: [],
      productitems: [],
      selectedBrand: {},
      selectedItem: {},
      dialogStatus: "",
      itemSelected: false,
      qty: 0,
      extended: "",
      strMSRP: "",
      cart: [], 
    });
    const loadBrands = async () => {
      try {
        state.status = "finding brands ...";
        const payload = await fetcher(`Brands`);
        if (payload.error === undefined) {
          state.brands = payload;
          state.status = `loaded ${state.brands.length} brands`;
        } else {
          state.status = payload.error;
        }
      } catch (err) {
        console.log(err);
        state.status = `Error has occured: ${err.message}`;
      }
    };
    const loadProductitems = async () => {
      try {
        state.status = `finding productitems for brand ${state.selectedBrand}...`;
        let payload = await fetcher(`Products/${state.selectedBrand}`);
        state.productitems = payload;
        state.status = `loaded ${state.productitems.length} product items`;
      } catch (err) {
        console.log(err);
        state.status = `Error has occured: ${err.message}`;
      }
      if (sessionStorage.getItem("cart")) {
        state.cart = JSON.parse(sessionStorage.getItem("cart"));
      }
    };
    const onRowSelect = (event) => {
      try {
        state.selectedItem = event.data;
        state.dialogStatus = "";
        state.itemSelected = true;
      } catch (err) {
        console.log(err);
        state.status = `Error has occured: ${err.message}`;
      }
    };
    const addToCart = () => {
      const index = state.cart.findIndex(
        // is item already on the cart
        (item) => item.id === state.selectedItem.id
      );
      if (state.qty !== 0) {
        index === -1 // add
          ? state.cart.push({
              id: state.selectedItem.id,
              qty: state.qty,
              item: state.selectedItem,
              extended: `${formatCurrency(
                state.qty * state.selectedItem.msrp
              )}`,
              strMSRP: `${formatCurrency(state.selectedItem.msrp)}`,
            })
          : (state.cart[index] = {
              // replace
              id: state.selectedItem.id,
              qty: state.qty,
              item: state.selectedItem,
              extended: `${formatCurrency(
                state.qty * state.selectedItem.msrp
              )}`,
              strMSRP: `${formatCurrency(state.selectedItem.msrp)}`,
            });
        state.dialogStatus = `${state.qty} item(s) added`;
      } else {
        index === -1 ? null : state.cart.splice(index, 1); // remove
        state.dialogStatus = `item(s) removed`;
      }
      sessionStorage.setItem("cart", JSON.stringify(state.cart));
      state.qty = 0;
    };
    const formatCurrency = (value) => {
      return value.toLocaleString("en-US", {
        style: "currency",
        currency: "USD",
      });
    };
    const router = useRouter();
    const viewCart = () => {
      router.push("cart");
    };
    return {
      state,
      loadProductitems,
      onRowSelect,
      addToCart,
      formatCurrency,
      viewCart,
    };
  },
};
</script>
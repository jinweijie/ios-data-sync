<template>
  <section class="section todo">
    <div class="container is-max-desktop">
      <h1 class="title">iCloud Reminders</h1>
      <nav class="panel">
        <p class="panel-heading">{{reminder.name}}</p>
        <label class="panel-block">
          <div class="column p-0 pl-4 is-four-fifths">Buy food</div>
          <div class="column p-0 pr-4 has-text-right">2021-08-09 19:00</div>
        </label>


        <label class="panel-block" v-for="item in reminder.items" :key="item.subject">
          <div class="column p-0 pl-4 is-four-fifths">{{item.subject}}</div>
          <div class="column p-0 pr-4 has-text-right">{{item.dueDate}}</div>
        </label>
        
      </nav>
    </div>
  </section>
</template>

<script>
import { ref } from 'vue'

export default {
  
  setup() {
    const reminder = ref({})
    reminder.value.name = 'my-iphone'
    reminder.value.items = [{'subject':'bb', 'dueDate':'2020-08-09 19:00'}, {'subject':'cc', 'dueDate':'2020-08-09 19:00'}]

    function fetchReminder() {
      fetch('http://10.10.10.11:5001/reminder/test')
        .then(res => res.json())
        .then(reminder => {
          console.log(reminder)
        })
    }

    return {
      reminder,
      fetchReminder
    }
  },
  mounted() {
    this.fetchReminder()
  }
};
</script>

<style>
.todo {
    height: 100vh;
    background: #cbd7e3;
}

.panel-block {
    background: #fff;
}
</style>

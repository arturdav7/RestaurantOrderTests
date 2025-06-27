# language: en
Feature: Checkout total calculation

@tag1
  Scenario: Four people order, each orders 1 starter, 1 main, and 1 drink at 15:00
    Given the order is 4 starters, 4 mains, 4 drinks at "15:00"
    When I calculate the total
    Then the total should be 55.40

@tag2
  Scenario: Two people order before 19:00, and two more join at 20:00 to add items
    Given the order is 1 starter, 2 mains, 2 drinks at "18:30"
    When I calculate the total and store it temporarily
    Then the intermediate total should be 23.30

    When the party grows to 4 people at "20:00" and 2 mains and 2 drinks are added
    And I calculate the total
    Then the total should be 43.70

@tag3
  Scenario: Four people order, then one cancels 1 starter, 1 main, and 1 drink
    Given the order is 4 starters, 4 mains, 4 drinks at "14:00"
    When I calculate the total
    Then the total should be 55.40

    When a member cancels 1 starter, 1 main, and 1 drink
    And I calculate the total
    Then the total should be 41.55

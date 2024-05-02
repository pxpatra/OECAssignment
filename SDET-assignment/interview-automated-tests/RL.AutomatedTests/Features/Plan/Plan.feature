Feature: Plan feature
    Test creating and adding procedures to a plan

    # example
    Scenario: Create Plan
        Given I'm on the start page
        When I click on start
        Then I'm on the plan page
        Then I'm adding procedure
        Then I'm assigning user
        Then I'm refereshing the page
        Then I'm checking the user name

# Expected test
# Scenario: Test Adding user to a plan procedure